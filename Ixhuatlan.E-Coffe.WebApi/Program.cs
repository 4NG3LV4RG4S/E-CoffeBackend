using Ixhuatlan.E_Coffe.WebApi.Configurations;

WebApplication.CreateBuilder(args)
    .ConfigurationServices()
    .ConfigureMiddlewares()
    .Run();
