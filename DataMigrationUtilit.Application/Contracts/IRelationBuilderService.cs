namespace DataMigrationUtilit.Application.Contracts
{
    public interface IRelationBuilderService
    {
        Task BuildDepartmentRelations();
        Task BuildEmployeeRelations();
    }
}
