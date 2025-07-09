using AngleSharp.Dom;
using System.ComponentModel.DataAnnotations;
using VocabBuilder.Infrastructure.Entities.Vocab;
using VocabBuilder.Shared;
using VocabBuilder.Shared.Validation;
using static VocabBuilder.Shared.Enums;
using Icons = Microsoft.FluentUI.AspNetCore.Components.Icons;

namespace VocabBuilder.ViewModels.Vocab;

public class VocabDetailViewModel
{
    private string _word = string.Empty;
    private string _maskedWord = string.Empty;

    public int Id { get; set; }

    [Required(ErrorMessage = "Word must not be empty.")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Word must be between 1 and 200 characters long.")]
    [RegularExpression(@"^[a-zA-Z\s\-'\.]+$", ErrorMessage = "Word can only contain letters, spaces, hyphens, apostrophes, and periods.")]
    public string Word
    {
        get => _word;
        set
        {
            if (_word != value)
            {
                _word = value.Trim().ToLower();
                MaskedWord = WordMasker.MaskHybrid(_word);
            }
        }
    }

    [StringLength(200, ErrorMessage = "Masked word must not exceed 200 characters.")]
    [MaskedWordValidation]
    public string MaskedWord
    {
        get => _maskedWord;
        set => _maskedWord = value;
    }

    [Required(ErrorMessage = "Word type must not be empty.")]
    [StringLength(50, ErrorMessage = "Word type must not exceed 50 characters.")]
    public string WordType { get; set; } = string.Empty;

    [Required(ErrorMessage = "Meaning must not be empty.")]
    [StringLength(1000, MinimumLength = 2, ErrorMessage = "Meaning must be between 2 and 1000 characters long.")]
    public string Meaning { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Phonetic must not exceed 100 characters.")]
    [RegularExpression(@"^\/[^\/]*\/\s*$|^$", ErrorMessage = "Phonetic must be in the format /phonetic/ or left blank.")]
    public string Phonetic { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "Definition must not exceed 2000 characters.")]
    public string Definition { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Example must not exceed 1000 characters.")]
    public string Example { get; set; } = string.Empty;

    public List<InputFileViewModel> InputFiles { get; set; } = CreateDefaultInputFiles();

    public static VocabDetailViewModel FromEntity(VocabEntity entity)
    {
        var vm = new VocabDetailViewModel
        {
            Id = entity.Id,
            Word = entity.Word,
            WordType = entity.WordType,
            Meaning = entity.Meaning,
            Phonetic = StringHelper.WrapWithSlash(entity.Phonetic),
            Definition = entity.Definition,
            Example = entity.Example,
        };

        vm.InputFiles.FirstOrDefault(f => f.FileType == FileType.Audio)!.FilePath = entity.Audio;
        vm.InputFiles.FirstOrDefault(f => f.FileType == FileType.Image)!.FilePath = entity.Image;

        return vm;
    }

    public VocabDetailViewModel Clone(ScreenType screenType = ScreenType.Detail) => new()
    {
        Id = screenType == ScreenType.Add ? 0 : Id,
        Word = Word,
        WordType = WordType,
        Meaning = Meaning,
        Phonetic = Phonetic,
        Definition = Definition,
        Example = Example,
        MaskedWord = MaskedWord,
        InputFiles = InputFiles
    };

    public VocabEntity ToEntity() => new()
    {
        Id = Id,
        Word = Word,
        WordType = WordType,
        Meaning = Meaning,
        Phonetic = StringHelper.UnwrapSlashes(Phonetic),
        Definition = Definition,
        Example = Example,
        MaskedWord = MaskedWord,
        Audio = InputFiles.FirstOrDefault(x => x.FileType == FileType.Audio)?.FilePath,
        Image = InputFiles.FirstOrDefault(x => x.FileType == FileType.Image)?.FilePath,
    };

    public void Reset()
    {
        Word = WordType = Meaning = Phonetic = Definition = Example = string.Empty;
        InputFiles.ForEach(x => x.FilePath = string.Empty);
    }

    public void RemoveFile(FileType fileType)
    {
        var inputFile = InputFiles.FirstOrDefault(f => f.FileType == fileType);
        if (inputFile is not null)
            inputFile.FilePath = string.Empty;
    }

    private static List<InputFileViewModel> CreateDefaultInputFiles() => [
        new InputFileViewModel
        {
            FileType = FileType.Audio,
            Accept = "audio/*",
            AnchorId = "audio-file-input",
            MaxFileSizeInKB = 100,
            Icon = new Icons.Regular.Size32.Headphones(),
        },
        new InputFileViewModel
        {
            FileType = FileType.Image,
            Accept = "image/*",
            AnchorId = "image-file-input",
            MaxFileSizeInKB = 500,
            Icon = new Icons.Regular.Size32.Image(),
        }
    ];
}