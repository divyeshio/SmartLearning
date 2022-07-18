using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SmartLearning.Core.Helpers
{
  public class AllowedFileExtensionsAttribute : ValidationAttribute
  {
    private readonly string[] _extensions;
    public AllowedFileExtensionsAttribute(string[] extensions)
    {
      _extensions = extensions;
    }

    protected override ValidationResult IsValid(
    object value, ValidationContext validationContext)
    {
      var file = value as IFormFile;
      if (file != null)
      {
        var extension = Path.GetExtension(file.FileName);
        if (!_extensions.Contains(extension.ToLower()))
        {
          return new ValidationResult(GetErrorMessage());
        }
      }

      return ValidationResult.Success;
    }

    public string GetErrorMessage()
    {
      return $"This file format is not Allowed!";
    }
  }
}
