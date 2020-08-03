# Azure Analysis Server (AAS) connection demo

## Basic description
Demo about connecting to Azure Analysis Server via C# using different methods and performing classic queries. We will be working with classic [official libraries](https://docs.microsoft.com/en-us/analysis-services/client-libraries?view=asallproducts-allversions).

In order to connect to the AAS you will need an [Azure](https;//www.azure.com "Azure Homepage") account. You can follow the following [tutorial](https://docs.microsoft.com/en-us/azure/analysis-services/analysis-services-create-server "AAS tutorial").

We will be working with sample model Adventure Works Internet Sales. To add the model to the service, follow [this](https://docs.microsoft.com/en-us/azure/analysis-services/analysis-services-create-sample-model) tutorial.

[![add project model](https://docs.microsoft.com/en-us/azure/analysis-services/media/analysis-services-create-sample-model/aas-create-sample-new-model.png)](https://docs.microsoft.com/en-us/azure/analysis-services/analysis-services-create-sample-model)

## File structure
To run .NET classic framework, you will need [.NET Framework 4.8](https://dotnet.microsoft.com/download/dotnet-framework).

I do recommend using IDE ([Visual Studio](https://visualstudio.com) or [Rider](https://jetbrains.com/rider)) or [Visual Studio Code with C# extension](https://code.visualstudio.com/) to edit and run the demo.

### In .NET classic you have 2 projects:
1. **aas.client** - command line app, which tests the solution 
2. **aas.web.api.classic** - web api application, which hosts api for getting the data from Azure Analysis Server

![.NET classic](http://data.azuredemos.net/public/aas-demo-file-structure-classic.png)

Also, you can see the [docker file](https://github.com/bovrhovn/aas-demo/blob/classic/src/Dockerfileclassic) to run it in your container registry.

The main implementation is in [QueryController.cs](https://github.com/bovrhovn/aas-demo/blob/classic/src/aas.demo/aas.web.api.classic/Controllers/QueryController.cs), start there, and add proper configuration data in [config file](https://github.com/bovrhovn/aas-demo/blob/classic/src/aas.demo/aas.web.api.classic/Web.config) in order for code to work.

# Additional information
If you have any question, open pull request or an issue or ping me on [Twitter](https://twitter.com/bvrhovnik).
