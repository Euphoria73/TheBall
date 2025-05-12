using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TheBallContracts.DataModels;
using TheBallContracts.Exceptions;
using TheBallContracts.StoragesContracts;
using TheBallDatabase.Models;

namespace TheBallDatabase.Implementations
{
    internal class PostStorageContract : IPostStorageContract
    {
        private readonly TheBallDbContext _dbContext;
        private readonly Mapper _mapper;

        public PostStorageContract(TheBallDbContext dbContext)
        {
            _dbContext = dbContext;
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Post, PostDataModel>()
                    .ForMember(x => x.Id, x => x.MapFrom(src => src.PostId));
                cfg.CreateMap<PostDataModel, Post>()
                    .ForMember(x => x.Id, x => x.Ignore())
                    .ForMember(x => x.PostId, x => x.MapFrom(src => src.Id))
                    .ForMember(x => x.IsActual, x => x.MapFrom(src => true))
                    .ForMember(x => x.ChangeDate, x => x.MapFrom(src => DateTime.UtcNow));
            });
            _mapper = new Mapper(config);
        }

        public PostDataModel? GetElementById(string id)
        {
            try
            {
                return _mapper.Map<PostDataModel>(_dbContext.Posts.FirstOrDefault(x => x.PostId == id && x.IsActual));
            }
            catch (Exception ex)
            {
                _dbContext.ChangeTracker.Clear();
                throw new StorageException(ex);
            }
        }

        public PostDataModel? GetElementByName(string name)
        {
            try
            {
                return _mapper.Map<PostDataModel>(_dbContext.Posts.FirstOrDefault(x => x.PostName == name && x.IsActual));
            }
            catch (Exception ex)
            {
                _dbContext.ChangeTracker.Clear();
                throw new StorageException(ex);
            }
        }

        public List<PostDataModel> GetList(bool onlyActual = true)
        {
            try
            {
                var query = _dbContext.Posts.AsQueryable();
                if (onlyActual)
                {
                    query = query.Where(x => x.IsActual);
                }
                return [.. query.Select(x => _mapper.Map<PostDataModel>(x))];
            }
            catch (Exception ex)
            {
                _dbContext.ChangeTracker.Clear();
                throw new StorageException(ex);
            }
        }

        public List<PostDataModel> GetPostWithHistory(string postId)
        {
            try
            {
                return [.. _dbContext.Posts.Where(x => x.PostId == postId).Select(x => _mapper.Map<PostDataModel>(x))];
            }
            catch (Exception ex)
            {
                _dbContext.ChangeTracker.Clear();
                throw new StorageException(ex);
            }
        }

        public void AddElement(PostDataModel postDataModel)
        {
            try
            {
                _dbContext.Posts.Add(_mapper.Map<Post>(postDataModel));
                _dbContext.SaveChanges();
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException { ConstraintName: "IX_Posts_PostName_IsActual" })
            {
                _dbContext.ChangeTracker.Clear();
                throw new ElementExistsException("PostName", postDataModel.PostName);
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException { ConstraintName: "IX_Posts_PostId_IsActual" })
            {
                _dbContext.ChangeTracker.Clear();
                throw new ElementExistsException("PostId", postDataModel.Id);
            }
            catch (Exception ex)
            {
                _dbContext.ChangeTracker.Clear();
                throw new StorageException(ex);
            }
        }

        public void DelElement(string id)
        {
            try
            {
                var element = GetPostById(id) ?? throw new ElementNotFoundException(id);
                if (!element.IsActual)
                {
                    throw new ElementDeletedException(id);
                }
                element.IsActual = false;
                _dbContext.SaveChanges();
            }
            catch
            {
                _dbContext.ChangeTracker.Clear();
                throw;
            }
        }

        public void ResElement(string id)
        {
            try
            {
                var element = GetPostById(id) ?? throw new ElementNotFoundException(id);
                element.IsActual = true;
                _dbContext.SaveChanges();
            }
            catch
            {
                _dbContext.ChangeTracker.Clear();
                throw;
            }
        }

        public void UpdElement(PostDataModel postDataModel)
        {
            try
            {
                var transaction = _dbContext.Database.BeginTransaction();
                try
                {
                    var element = GetPostById(postDataModel.Id) ?? throw new ElementNotFoundException(postDataModel.Id);
                    if (!element.IsActual)
                    {
                        throw new ElementDeletedException(postDataModel.Id);
                    }
                    element.IsActual = false;
                    _dbContext.SaveChanges();
                    var newElement = _mapper.Map<Post>(postDataModel);
                    _dbContext.Posts.Add(newElement);
                    _dbContext.SaveChanges();
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException { ConstraintName: "IX_Posts_PostName_IsActual" })
            {
                _dbContext.ChangeTracker.Clear();
                throw new ElementExistsException("PostName", postDataModel.PostName);
            }
            catch (Exception ex) when (ex is ElementDeletedException || ex is ElementNotFoundException)
            {
                _dbContext.ChangeTracker.Clear();
                throw;
            }
            catch (Exception ex)
            {
                _dbContext.ChangeTracker.Clear();
                throw new StorageException(ex);
            }
        }

        private Post? GetPostById(string id) => _dbContext.Posts.Where(x => x.PostId == id).OrderByDescending(x => x.ChangeDate).FirstOrDefault();
    }
}
