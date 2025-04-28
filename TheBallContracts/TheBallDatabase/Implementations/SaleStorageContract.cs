
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TheBallContracts.DataModels;
using TheBallContracts.Exceptions;
using TheBallContracts.StoragesContracts;
using TheBallDatabase.Models;

namespace TheBallDatabase.Implementations;

internal class SaleStorageContract : ISaleStorageContract
{
    private readonly TheBallDbContext _dbContext;
    private readonly Mapper _mapper;

    public SaleStorageContract(TheBallDbContext dbContext)
    {
        _dbContext = dbContext;
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<SaleGift, SaleGiftDataModel>();
            cfg.CreateMap<SaleGiftDataModel, SaleGift>();
            cfg.CreateMap<Sale, SaleDataModel>();
            cfg.CreateMap<SaleDataModel, Sale>()
                .ForMember(x => x.IsCancel, x => x.MapFrom(src => false))
                .ForMember(x => x.SaleGifts, x => x.MapFrom(src => src.Gifts));
        });
        _mapper = new Mapper(config);
    }
  
    public SaleDataModel? GetElementById(string id)
    {
        try
        {
            return _mapper.Map<SaleDataModel>(GetSaleById(id));
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public List<SaleDataModel> GetList(DateTime? startDate = null, DateTime? endDate = null, string? workerId = null, string? buyerId = null, string? giftId = null)
    {
        try
        {
            var query = _dbContext.Sales.Include(x => x.SaleGifts).AsQueryable();
            if (startDate is not null && endDate is not null)
            {
                query = query.Where(x => x.SaleDate >= startDate && x.SaleDate < endDate);
            }
            if (workerId is not null)
            {
                query = query.Where(x => x.WorkerId == workerId);
            }
            if (buyerId is not null)
            {
                query = query.Where(x => x.BuyerId == buyerId);
            }
            if (giftId is not null)
            {
                query = query.Where(x => x.SaleGifts!.Any(y => y.GiftId == giftId));
            }
            return [.. query.Select(x => _mapper.Map<SaleDataModel>(x))];
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public void AddElement(SaleDataModel saleDataModel)
    {
        try
        {
            _dbContext.Sales.Add(_mapper.Map<Sale>(saleDataModel));
            _dbContext.SaveChanges();
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
            var element = GetSaleById(id) ?? throw new ElementNotFoundException(id);
            if (element.IsCancel)
            {
                throw new ElementDeletedException(id);
            }
            element.IsCancel = true;
            _dbContext.SaveChanges();
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

    private Sale? GetSaleById(string id) => _dbContext.Sales.FirstOrDefault(x => x.Id == id);
}
