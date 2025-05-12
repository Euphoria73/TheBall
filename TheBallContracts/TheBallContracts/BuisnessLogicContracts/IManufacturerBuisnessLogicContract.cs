
using TheBallContracts.DataModels;

namespace TheBallContracts.BuisnessLogicContracts;

public interface IManufacturerBuisnessLogicContract
{
    List<ManufacturerDataModel> GetAllManufacturers();

    ManufacturerDataModel GetManufacturerByData(string data);

    void InsertManufacturer(ManufacturerDataModel manufacturerDataModel);

    void UpdateManufacturer(ManufacturerDataModel manufacturerDataModel);

    void DeleteManufacturer(string id);
}
