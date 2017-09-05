using System;
using System.Linq;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ImageGenerator
{
	public class FileOperation : IOperationFilter
	{
		public void Apply(Operation operation, OperationFilterContext context)
		{
			if (operation.OperationId.ToLower() == "post-imagepost")
			{
                //operation.Parameters.Clear();//Clearing parameters

                var file = operation.Parameters.Where(x => x.Name == "file").FirstOrDefault();
                if (file != null) {
                    operation.Parameters.Remove(file);
                }

				operation.Parameters.Add(new NonBodyParameter
				{
					Name = "file",
					In = "formData",
					Description = "Uplaod Image",
					Required = true,
					Type = "file"
				});
				operation.Consumes.Add("application/form-data");
			}
		}
	}
}
