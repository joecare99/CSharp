# libCIFAR

`libCIFAR` models the fixed-size binary records used by CIFAR-10 and
CIFAR-100. Each record contains a label and planar RGB channels for a 32x32
image. `GetImageAsRgbArray` converts those channels into interleaved RGB
triplets suitable for image consumers.

## Tests

`libCIFAR.Tests` uses synthetic records to verify label mapping, CIFAR-100
subcategories, stream loading, and planar-to-interleaved RGB conversion.

```powershell
dotnet test C:\Projekte\CSharp\CSharpBible\Graphics\libCIFAR.Tests\libCIFAR.Tests.csproj --configuration Debug
```
