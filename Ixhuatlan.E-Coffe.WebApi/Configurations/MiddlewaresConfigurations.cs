namespace Ixhuatlan.E_Coffe.WebApi.Configurations;

internal static class MiddlewaresConfigurations
{
    public static WebApplication ConfigureMiddlewares(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseDeveloperExceptionPage();

        app.UseSwagger();
        app.UseSwaggerUI(option =>
        {
            option.SwaggerEndpoint("/swagger/v1/swagger.json", "Ixhuatlan.E-Coffe.WebApi. v1");
        });

        app.UseRouting();
        app.UseAuthorization();

        app.UseResponseCompression();
        app.UseHttpsRedirection();
        app.MapControllers();

        app.UseEndpoints(configure: endpoints => endpoints.MapControllers());

        app.UseStatusCodePages();

        app.UseCors(option =>
        {
            option.AllowAnyOrigin();
            option.AllowAnyMethod();
            option.AllowAnyHeader();
        });

        return app;
    }
}