## Design and architecture

### Domain model

Provide an illustration of your domain model.
Make sure that it is correct and complete.
In case you are using ASP.NET Identity, make sure to illustrate that accordingly.

### Architecture — In the small

Illustrate the organization of your code base.
That is, illustrate which layers exist in your (onion) architecture.
Make sure to illustrate which part of your code is residing in which layer.

### Architecture of deployed application

Illustrate the architecture of your deployed application.
Remember, you developed a client-server application.
Illustrate the server component and to where it is deployed, illustrate a client component, and show how these communicate with each other.

### User activities

Illustrate typical scenarios of a user journey through your _Chirp!_ application.
That is, start illustrating the first page that is presented to a non-authorized user, illustrate what a non-authorized user can do with your _Chirp!_ application, and finally illustrate what a user can do after authentication.

Make sure that the illustrations are in line with the actual behavior of your application.

### Sequence of functionality/calls trough _Chirp!_

With a UML sequence diagram, illustrate the flow of messages and data through your _Chirp!_ application.
Start with an HTTP request that is send by an unauthorized user to the root endpoint of your application and end with the completely rendered web-page that is returned to the user.

Make sure that your illustration is complete.
That is, likely for many of you there will be different kinds of "calls" and responses.
Some HTTP calls and responses, some calls and responses in C# and likely some more.
(Note the previous sentence is vague on purpose. I want that you create a complete illustration.)

## Process

### Build, test, release, and deployment

Illustrate with a UML activity diagram how your _Chirp!_ applications are build, tested, released, and deployed.
That is, illustrate the flow of activities in your respective GitHub Actions workflows.

Describe the illustration briefly, i.e., how your application is built, tested, released, and deployed.

### Team work

Show a screenshot of your project board right before hand-in.
Briefly describe which tasks are still unresolved, i.e., which features are missing from your applications or which functionality is incomplete.

Briefly describe and illustrate the flow of activities that happen from the new creation of an issue (task description), over development, etc. until a feature is finally merged into the `main` branch of your repository.

### How to make _Chirp!_ work locally
In order to run our project locally follow these steps:
1. Firstly install the .NET SDK-version `.NET 8.0`.
2. Clone our Git repository with: \
`git clone https://github.com/ITU-BDSA2025-GROUP26/Chirp.git` \
The folder should now contain (among other things): 
    1. `src` 
    2. `test`
    3. `Chirp.Razor.sln`. 
3. To run the project `cd` into `src/Chirp.Web` and from here run: `dotnet run`.

After the last step there should be the following two links: 
1. Now listening on: https://localhost:7102
2. Now listening on: http://localhost:5273 

Pressing either of the links will redirect to the homepage of our application and use HTTPS. To close the application press Ctrl + C in the terminal window.

### How to run test suite locally
In our Chirp-project we have two test suites: Playwright for end-to-end tests and XUnit tests for unit- and integrationtests. For both Playwright and XUnit tests, we have created a test-folder under `Chirp/test`. \
\
To run the XUnit test, the user needs `.NET 8.0`. Afterwards the user will need to change the directory to the folder containing a `.csproj` file, and then run `dotnet test`. \
For example to run the integration tests regarding our infrastructure, the user will need to cd into `Chirp/test/IntegrationTest/Chirp.InfrastructureTests` and then run `dotnet test`. \
Beware that some folders are nested folders to keep a more clean folder-hierarchy, for example the `UnitTest` folder have multiple folders respectively to core, infrastructure and web. \
To run all the tests in the project you can run `dotnet test` under the root-folder `Chirp`. This includes Playwright tests which will give errors, if the program is not runnning in another terminal window. \
\
To run the end-to-end tests the user will firstly need to have playwright installed:
1. cd into `test/PlaywrightTests/PlaywrightTest`
2. Run `dotnet add package Microsoft.Playwright.NUnit --version 1.43.0`
3. Build the project with `dotnet build`
4. Finally run `dotnet playwright install`

To run the Playwright tests it is a requirement that the project is running in its own terminal, so the user will have to run project with `dotnet run` in `src/Chirp.Web`. Afterwards in a new terminal the user needs to cd into `test/PlaywrightTests/PlaywrightTest` and then run `dotnet test`.

**Test suite** \
Describing which types of tests we have and what they are testing


## Ethics

### License

We have chosen the MIT License for Chirp. MIT license is a short and direct license, that gives us alot of freedom with the project. \
When we needed to chose a license for our project, we were quite in doubt about which license to chose. Therefore we went to 'choosealicense.com and  after some consideration we chose the MIT license. We also confirmed that the dependencies in our .csproj file were compatible with the MIT license.\
The MIT license-file can be found under 'Chirp/LICENSE'.

### LLMs, ChatGPT, CoPilot, and others

State which LLM(s) were used during development of your project.
In case you were not using any, just state so.
In case you were using an LLM to support your development, briefly describe when and how it was applied.
Reflect in writing to which degree the responses of the LLM were helpful.
Discuss briefly if application of LLMs sped up your development or if the contrary was the case.\
Some of 

LLMs used:
ChatGPT
CoPilot
Claude

Used for:
CLI commands.
HTML code.
Workflows.

- [ ] "Design and architecture"
    - [ ] "Domain model"
    - [ ] "Architecture — In the small"
    - [ ] "Architecture of deployed application"
    - [ ] "User activities"
    - [ ] "Sequence of functionality/calls through Chirp!"
- [ ] "Process"
    - [ ] "Build, test, release, and deployment"
    - [ ] "Team work"
    - [x] "How to make Chirp! work locally"
    - [ ] "How to run test suite locally"
- [ ] "Ethics"
    - [x] "License"
    - [ ] "LLMs, ChatGPT, CoPilot, and others"