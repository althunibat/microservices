using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Framework.Api
{
    public class SwaggerDefaultValues : IOperationFilter {
        /// <summary>
        /// Applies the filter to the specified operation using the given context.
        /// </summary>
        /// <param name="operation">The operation to apply the filter to.</param>
        /// <param name="context">The current operation filter context.</param>
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                return;

            var fileParamNames = context.ApiDescription.ActionDescriptor.Parameters
                .SelectMany(x => x.ParameterType.GetProperties())
                .Where(x => x.PropertyType.IsAssignableFrom(typeof (IFormFile)))
                .Select(x => x.Name)
                .ToList();
            if (!fileParamNames.Any())
            {
                // REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/412
                // REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/413
                foreach (var parameter in operation.Parameters.OfType<NonBodyParameter>()) {
                    var description = context.ApiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);

                    if (parameter.Description == null) {
                        parameter.Description = description.ModelMetadata.Description;
                    }

                    if (parameter.Default == null) {
                        parameter.Default = description.RouteInfo.DefaultValue;
                    }

                    parameter.Required |= !description.RouteInfo.IsOptional;
                }
                return;
            }

            var paramsToRemove = new List<IParameter>();
            foreach (var param in operation.Parameters)
            {
                paramsToRemove.AddRange(from fileParamName in fileParamNames where param.Name.StartsWith(fileParamName + ".") select param);
            }
            paramsToRemove.ForEach(x => operation.Parameters.Remove(x));
            foreach (var paramName in fileParamNames)
            {
                var fileParam = new NonBodyParameter
                {
                    Type = "file",
                    Name = paramName,
                    In = "formData"
                };
                operation.Parameters.Add(fileParam);
            }
            foreach (var param in operation.Parameters)
            {
                param.In = "formData";
            }

            operation.Consumes = new List<string>() { "multipart/form-data" };
        }
    }
    
}