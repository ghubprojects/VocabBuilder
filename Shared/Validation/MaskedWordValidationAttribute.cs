using System.ComponentModel.DataAnnotations;
using VocabBuilder.ViewModels.Vocab;

namespace VocabBuilder.Shared.Validation;

public class MaskedWordValidationAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var maskedWord = value as string;
        if (string.IsNullOrWhiteSpace(maskedWord))
            return ValidationResult.Success;

        var vocabDetailModel = (VocabDetailViewModel)validationContext.ObjectInstance;
        if (string.IsNullOrWhiteSpace(vocabDetailModel.Word))
            return ValidationResult.Success;

        var word = vocabDetailModel.Word.Trim();

        if (!maskedWord.Contains('_'))
        {
            return new ValidationResult(
                "Masked word must contain at least one '_' character to hide part of the word.",
                [validationContext.MemberName!]);
        }

        if (maskedWord.Length != word.Length)
        {
            return new ValidationResult(
                "Masked word must have the same length as the original word.",
                [validationContext.MemberName!]);
        }

        for (int i = 0; i < word.Length; i++)
        {
            var originalChar = char.ToLower(word[i]);
            var maskedChar = char.ToLower(maskedWord[i]);
            if (maskedChar != '_' && maskedChar != originalChar)
            {
                return new ValidationResult(
                    "Unmasked characters must match the corresponding letters in the original word.",
                    [validationContext.MemberName!]);
            }
        }

        return ValidationResult.Success;
    }
}