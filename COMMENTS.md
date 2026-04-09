--Comments
-I requested claude for fixing the found issues. After that, the solutions were fully compiled
-I requested Claude for update claude.md with the current version of this system, and generate readme.md explaining how to run the application, how to use the seed users to connect to the website, and participate in an auction 
-After launching the application and verified website, I realized that the buyer card for performing a bid on a vehicle auction was not showing. Requested a fix by typing to Claude: I could not type the new bid for the auction. Make it work as the instructions of the readme mention: "Enter your bid amount and click **Place Bid**. The new price is reflected immediately." The buyer should be able to
The cause was a response value of active/inactive that should be 1/0 for the auction and buyer role that should be 1 instead of buyer

What I would do/did differently
-I updated the controller files location to controllers/*
-I would utilize const and private readonly string for hardcoded values
-Cors accepts requests from any origin currently, which is ok for testing, but in production, this can be updated
-I would update log level accordingly in the code with Log.Debug (flow validation); Log.Information (business rules); Log.Warning (fallback); Log.Error (original exceptions)
-I would use a Mapper for converting the entity to the response object
-I could have different assemblies for features and shared functionalities
-I would add a total_price field at the auction entity that would contain the bid price value + all fees
-I would add a real database
-I would configure for Kubernetes utilization

--Run Details
-If you run outside Docker, then you need to update the frontend .env.development file or .env.production file to reach the backend api accordingly, as it has a different port from docker version
-Docker runs fine (it was configured to run in a Linux container)
