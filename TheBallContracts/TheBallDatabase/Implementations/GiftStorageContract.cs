using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TheBallContracts.DataModels;
using TheBallContracts.Exceptions;
using TheBallContracts.StoragesContracts;
using TheBallDatabase.Models;

namespace TheBallDatabase.Implementations;

internal class GiftStorageContract : IGiftStorageContract
{
    private readonly TheBallDbContext _dbContext;
    private readonly Mapper _mapper;

    public GiftStorageContract(TheBallDbContext dbContext)
    {
        _dbContext = dbContext;
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Gift, GiftDataModel>();
            cfg.CreateMap<GiftDataModel, Gift>()
                .ForMember(x => x.IsDeleted, x => x.MapFrom(src => false));
            cfg.CreateMap<GiftHistory, GiftHistoryDataModel>();
        });
        _mapper = new Mapper(config);
    }
        
    public GiftDataModel? GetElementById(string id)
    {
        try
        {
            return _mapper.Map<GiftDataModel>(GetGiftById(id));
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public GiftDataModel? GetElementByName(string name)
    {
        try
        {
            return _mapper.Map<GiftDataModel>(_dbContext.Gifts.FirstOrDefault(x => x.Name == name && !x.IsDeleted));
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public List<GiftHistoryDataModel> GetHistoryByGiftId(string giftId)
    {
        try
        {
            return [.. _dbContext.GiftHistories.Where(x => x.GiftId == giftId).OrderByDescending(x => x.ChangeDate).Select(x => _mapper.Map<GiftHistoryDataModel>(x))];
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public List<GiftDataModel> GetList(bool onlyActive = true, string? manufacturerId = null)
    {
        try
        {
            var query = _dbContext.Gifts.AsQueryable();
            if (onlyActive)
            {
                query = query.Where(x => !x.IsDeleted);
            }
            if (manufacturerId is not null)
            {
                query = query.Where(x => x.ManufacturerId == manufacturerId);
            }
            return [.. query.Select(x => _mapper.Map<GiftDataModel>(x))];
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }
    public void AddElement(GiftDataModel giftDataModel)
    {
        try
        {
            _dbContext.Gifts.Add(_mapper.Map<Gift>(giftDataModel));
            _dbContext.SaveChanges();
        }
        catch (InvalidOperationException ex) when (ex.TargetSite?.Name == "ThrowIdentityConflict")
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException("Id", giftDataModel.Id);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { ConstraintName: "IX_Gifts_GiftName_IsDeleted" })
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException("GiftsName", giftDataModel.Name);
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
            var element = GetGiftById(id) ?? throw new ElementNotFoundException(id);
            element.IsDeleted = true;
            _dbContext.SaveChanges();
        }
        catch (ElementNotFoundException ex)
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

    public void UpdElement(GiftDataModel giftDataModel)
    {
        try
        {
            var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                var element = GetGiftById(giftDataModel.Id) ?? throw new ElementNotFoundException(giftDataModel.Id);
                if (element.Price != giftDataModel.Price)
                {
                    _dbContext.GiftHistories.Add(new GiftHistory() { GiftId = element.Id, OldPrice = element.Price });
                    _dbContext.SaveChanges();
                }
                _dbContext.Gifts.Update(_mapper.Map(giftDataModel, element));
                _dbContext.SaveChanges();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { ConstraintName: "IX_Products_ProductName_IsDeleted" })
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException("ProductName", giftDataModel.Name);
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

    private Gift? GetGiftById(string id) => _dbContext.Gifts.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
}
