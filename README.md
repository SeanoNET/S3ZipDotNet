# S3ZipSharp [![Build Status](https://travis-ci.org/SeanoNET/S3ZipSharp.svg?branch=master)](https://travis-ci.org/SeanoNET/S3ZipSharp)
A tool to retrieve and zip objects from a s3 bucket and zip them up, uploading the zip file back into s3

## Getting Started

Install the [NuGet package](https://www.nuget.org/packages/S3ZipSharp/)

`Install-Package S3ZipSharp`

These instructions will get your clone of S3ZipSharp up and running on your local machine for development.

- Download and install [.NET Core 3.1+](https://dotnet.microsoft.com/download) 
- `cd /S3ZipSharp/`
- `dotnet restore`
- `dotnet build`

### Configure 

| | Description|
|---|---|
| AccessKeyId | AWS access key |
| SecretAccessKey | AWS secret key |
| AwsRegion | AWS region |
| S3BucketName | Name of the S3 bucket |
| BatchSize | Amount of objects to download an zip in parallel, reduce to improve memory footprint |
| TempZipPath | Location of the temp zip file |
| TempZipDir | Path to the temp zip file |
| CompressionLevel | Compression level when zipping files |

```csharp
var config = new Config(
    "AccessKey", 
    "SecretAccessKey", 
    "Region", 
    "my-s3-bucket")
```

To use `appsettings.json` install [Microsoft.Extensions.Configuration](https://www.nuget.org/packages/Microsoft.Extensions.Configuration/) nuget package 

```
Install-Package Microsoft.Extensions.Configuration 
```

Configure AWS credentials in `appsettings.json`

```csharp
{
  "Aws": {
    "AccessKey": "AccessKey",
    "SecretAccessKey": "SecretAccessKey",
    "Region": "Region"
  }
}
```

```csharp
// Load from configuration
var cfg = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

S3ZipSharp objectsZipper = new S3ZipSharp(
    new Models.Config(cfg.GetSection("Aws")["AccessKey"], cfg.GetSection("Aws")["SecretAccessKey"], 
    cfg.GetSection("Aws")["Region"], 
    "my-s3-bucket"));
```

Example for zipping a folder called `documents` in bucket `my-s3-bucket`.

```csharp
await objectsZipper.ZipBucket("documents", "documents.zip", new System.Threading.CancellationToken());
```

## Filtering out files

If you wanted to ignore pdf files when zipping, you can include a filterOutFiles parameter.

```csharp
//key is the name of the file or object on s3.
objectsZipper.filterOutFiles = (key) =>
{
    return !key.Contains(".pdf");
};
```

### Full Example

See [Program.cs](S3ZipSharp.Example/Program.cs) for a full example.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details