﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SmartLearning.Core.Helpers
{
  public class MaxFileSizeAttribute : ValidationAttribute
  {
    private readonly int _maxFileSize;
    public MaxFileSizeAttribute(int maxFileSize)
    {
      _maxFileSize = maxFileSize;
    }

    protected override ValidationResult IsValid(
    object value, ValidationContext validationContext)
    {
      var file = value as IFormFile;
      if (file != null)
      {
        if (file.Length > _maxFileSize)
        {
          return new ValidationResult(GetErrorMessage());
        }
      }

      return ValidationResult.Success;
    }

    public string GetErrorMessage()
    {
      return $"Maximum allowed file size is {_maxFileSize / 1024 / 1024} mb.";
    }
  }
}
