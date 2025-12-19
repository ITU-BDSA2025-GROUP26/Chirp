---
title: _Chirp!_ Project Report
subtitle: ITU BDSA 2025 Group `26`
author:
- "Nanna Helge <nanhe@itu.dk>"
- "Victor Hvid Troelsen <vitr@itu.dk>"
- "Nick Kjær Christoffersen <nkch@itu.dk>"
- "David Ronaldo Kijewska Kocuba <dkoc@itu.dk>"
- "Carl Philip Frederik Ferraro Steuch <caps@itu.dk>"
numbersections: true
---
## Design and architecture

### Domain model
The illustration shows our domain model and its dependencies. The domain model is used by ASP .NET's EF Core package to construct a relational database in SQLite containing authors, the cheeps they write, and how to like cheeps. Moreover, the `Author` class depends on ASP .NET's Identity package, specifically it inherits from `IdentityUser`, including multiple fields such as `UserName`, `Email` and `Id`. We chose not to overwrite the `UserName` field from `IdentityUser` and used it as a display name in the application as well. \
This combination of Identity and EF Core allows our application to have users (Authors) who can login, and see the Cheeps they have written and more.\
![Domain Model](diagrams/png/Domain%20Model.png)


### Architecture — In the small
The onion architecture structure helps us to separate backend logic (domain model, database queries etc.) from frontend logic (layout, HTTP methods such as GET and POST etc.), and it provides a clear structure to where each class should be placed. \
As seen in the illustration our code is structured after the Onion Architecture, and the code base is using the Repository Pattern.
![Onion Architecture](diagrams/png/Simplified%20Onion%20Architecture.png) \
The above diagram is a general and simplistic model of our onion architecture. We have also created a more detailed overview of the onion archittecture in the following UML-diagram. \
![UML Onion Architecture](diagrams/png/UML%20OnionArchitecture.png)

### Architecture of deployed application
Our Chirp application is a client-server application, meaning clients send HTTP requests to the server, which is then responsible for rendering the web pages, updating and persisting data changes in the database and redirection of the client through the application when needed. Our webserver is hosted by Azure App Service. The illustration shows SQLite as a separate component which is probably a bit misleading. In reality our database is a `db` file within our application, but we wanted to include the database when illustrating the deployed application.\
![Deployed Application](diagrams/png/Deployed%20Application.png)

### User activities
**Register and Login** \
The model shows the progression of a user from viewing the public timeline as a non-authenticated user to gaining access to other features such as follow, unfollow and like. The diagram shows the two different paths for either registration and login before being granted access to features. \
![Register and Login](diagrams/png/User%20activites%20-%20Register%20and%20Login%.png)

**Create new Cheep** \
This illustration shows a client's journey from the public timeline as a non-authenticated user to an authenticated user which can post Cheeps. A user can also post Cheeps from the user timeline page but is not shown in the diagram.\
![Create new Cheep](/diagrams/png/User%20activites%20-%20Make%20cheep.png)

**About Me and Delete Me** \
The flow starts by displaying the `public timeline`. The user navigates to the `login page`  and logs in, after which the `public timeline` is shown again, now with authenticated functionality enabled. From there, the user can navigate to the `About Me` page, where their `username`, `email`, `cheeps` and `follower/following` counts are displayed. \
From the `About Me` page, the user can choose to use the `Forget Me` functionality. This action triggers a confirmation alert to prevent accidental deletion. Once the deletion is confirmed, the user account is removed and the application returns to displaying the `public timeline` in a non-authenticated state. \
![About Me and Delete Me](/diagrams/png/User%20activites%20-%20About%20Me%20and%20Delete.png)

### Sequence of functionality/calls trough _Chirp!_

With a UML sequence diagram, illustrate the flow of messages and data through your _Chirp!_ application.
Start with an HTTP request that is send by an unauthorized user to the root endpoint of your application and end with the completely rendered web-page that is returned to the user.

Make sure that your illustration is complete.
That is, likely for many of you there will be different kinds of "calls" and responses.
Some HTTP calls and responses, some calls and responses in C# and likely some more.
(Note the previous sentence is vague on purpose. I want that you create a complete illustration.)

The illustration shows HTTP calls from a client to the web server for both authenticated and non-authenticated users. \
![Sequence Diagram](/diagrams/png/SequenceDiagram.png)

## Process

### Build, test, release, and deployment
As seen in the illustration, the workflows are triggered on push and pull requests. It starts with the first workflow, `dotnet.yml`, which goes basic steps such as restoring dependencies, building the project, installing Playwright and finishes with running unit - and integration tests. The tests need to pass in order to achieve a successful workflow run. \
If the workflows are triggered on the `main` branch, GitHub Action runner will continue to `release.yml`. This workflow repeats restoring dependencies and building the solution. Afterwards it will prepare for deployment by publishing and uploading the web app. If the push is tagged, the workflow will also create a release. `release.yml` finishes by deploying the web app to Azure's App Service.\
![Workflows](/diagrams/png/Workflows.png) \
Our worflows ensure that no matter the branch, the code is always at least restored, built and tested when pushing to GitHub. Furthermore, we have configured our `release.yml` to only run on our `main` branch, meaning only production ready code was deployed to our Azure site. 

**Issues with workflows** \
We also made a workflow named `issue-labelling.yml`. The goal was to use the workflow to automatically move our issues across the project board, but we had issues making it work, and it was quickly discarded. We should have probably just deleted the workflow. \
Lastly, we encountered an issue with our `release.yml` workflow when we created a new tag, as Azure would not let us login if there were a tag. This is an Azure side error and not an error with the workflow itself. We decided not to try and fix it, as we encountered the error late in the semester, and without the tag it will deploy normally.

### Team work
**Handling of mandatory features in a group setting**\
We have tried to model our process in the shown activity diagram. Our process was fairly consistent throughout the semester. When a new task or feature was introduced in the lecture, a group member would start on the task by creating a ticket for the task, and would then continue trying to develop this new feature or complete the task. It felt pretty natural for the members in the group to take on tasks during the project, as other members still might be working on the weekly task prior. If there were issues with implementing a feature or completing a task, the person working on it, would usually ask help from others in the group or ask a TA. When the task was done, a pull request was created and if approved, it would be merged to the `main` branch. Finally, the issue would be moved from In Progress to Done on the project board.
![Workflows](/diagrams/png/ProjectBoard.png)

**Unresolved issues** \
We managed to implement all required functionality of the project including a wild style feature, which we chose to be a like function. An issue we did not manage to solve was the `Fix like button sizing`. As the project is now, whenever a cheep is longer than two lines, the like button will resize accordingly, leading to an inconsistent layout. \
Lastly, we had an old issue which is still open called `Adjust test suites`. It has remained open because the issue has been relevant throughout the semester, as we have continuously implemented new features which would need testing.
![Workflows](/diagrams/png/Teamwork.png)

### How to make _Chirp!_ work locally
In order to run our project locally follow these steps:
1. Firstly install the .NET SDK-version `.NET 8.0`.
2. Clone our Git repository from the terminal with:
```bash
git clone https://github.com/ITU-BDSA2025-GROUP26/Chirp.git
``` 
The folder should now contain (among other things): `src`, `test`,`Chirp.Razor.sln` \
3. To allow registering with GitHub `cd` into `src/Chirp.Web` and run the three following commands in your terminal:
```bash
dotnet user-secrets init
dotnet user-secrets set "authentication_github_clientId" "Ov23li8tJVPjkWxP3PA0"
dotnet user-secrets set "authentication_github_clientSecret" "dbb574deaac6b57c60ad2d322fd84b9caf22f83d"
```
4. To ensure HTTPS `cd` into `src/Chirp.Web` and run the following in your terminal: 
```bash
dotnet dev-certs https --trust
```
6. To run the project `cd` into `src/Chirp.Web` and from here in the terminal run: 
```bash
dotnet run
```

After the last step there should be the following two links: 
1. Now listening on: https://localhost:7102
2. Now listening on: http://localhost:5273 

Pressing either of the links will redirect to the homepage of our application and use HTTPS. To close the application press Ctrl + C in the terminal window.

### How to run test suite locally
In our Chirp-project we have two test suites: Playwright for end-to-end tests and XUnit tests for unit- and integrationtests. For both Playwright and XUnit tests, we have created a test-folder under `Chirp/test`. \
\
To run the XUnit test, the user needs `.NET 8.0`. Afterwards the user will need to change the directory to the folder containing a `.csproj` file, and then run:
```bash
dotnet test
```
For example to run the integration tests regarding our infrastructure, the user will need to `cd` into `Chirp/test/IntegrationTest/Chirp.InfrastructureTests` and then run `dotnet test`. \
Beware that some folders are nested folders to keep a more clean folder-hierarchy, for example the `UnitTest` folder have multiple folders respectively to core, infrastructure and web. \
To run all the tests in the project you can run `dotnet test` under the root-folder `Chirp`. This includes Playwright tests which will give errors, if the program is not runnning in another terminal window. \
\
To run the end-to-end tests the user will firstly need to have Playwright installed:
1. `cd` into `test/PlaywrightTests/PlaywrightTest`
2. Run 
```bash
dotnet add package Microsoft.Playwright.NUnit --version 1.43.0
```
3. Build the project with
```bash
dotnet build
```
4. Finally run 
```bash
dotnet playwright install
```

To run the Playwright tests it is a requirement that the project is running in its own terminal, so the user will have to run project with `dotnet run` in `src/Chirp.Web`. Afterwards in a new terminal the user needs to cd into `test/PlaywrightTests/PlaywrightTest` and then run `dotnet test`.

**Test suite** \
We have tried to regulary write tests for the features that we implemented throughout the semester. This includes both unit tests (with XUnit), integration tests (using WebApplicationFactory from C#) and end-to-end tests (using Playwright). Moreover, we have tried to organise our tests, first into the type of test and then which layer of the Onion Architecture, the test class was testing. When running the code with dotCover in JetBrains Rider, it shows a coverage of 30%. This number should be higher, and next time we have to make a similar project, it will be a focus point to write more tests.


## Ethics

### License

We have chosen the MIT License for Chirp. MIT license is a short and direct license, that gives us alot of freedom with the project. \
When we needed to chose a license for our project, we were quite in doubt about which license to chose. Therefore we went to choosealicense.com and  after some consideration we chose the MIT license. We also confirmed that the dependencies in our .csproj file were compatible with the MIT license.\
The MIT license-file can be found under `Chirp/LICENSE`.

### LLMs, ChatGPT, CoPilot, and others
One member of the group did not use LLMs. The rest of the group did use LLMs. They were used for different things such as debugging code (both logic mistakes and syntax errors) and CLI commands. LLMs was also used for code syntax, as an example we had some issues with Playwright when writing a test for Login and Register (the compiler could not differentiate between "Password" and "Confirm password"), which an LLM quickly fixed for us by changing the syntax. \
Lastly, we used GitHub Actions to generate our workflows based on our repository and needs, which in turn used GitHub CoPilot. We then later adjusted the generated workflows to better fix our needs and requirements. \
LLMs were definitely useful in the above mentioned cases and greatly helped the speed of our development. It saved us a lot of time of debugging. LLMs are great as a complimentary tool, but only as a tool, as it might answer with something incorrect. \
A short list of the LLMs that we used: ChatGPT, GitHub CoPilot, Claude