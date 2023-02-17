// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

// <ProgramSnippet>
using GraphTutorial.AuthZ.Models;

Console.WriteLine(".NET Graph Tutorial\n");

var settings = Settings.LoadSettings();

// Initialize Graph
InitializeGraph(settings);

// Greet the user by name
await GreetUserAsync(AuthZPolicy.Base);

int choice = -1;

while (choice != 0)
{
    Console.WriteLine("Please choose one of the following options:");
    Console.WriteLine("0. Exit");
    Console.WriteLine("1. Allow all requests");
    Console.WriteLine("2. Allow users path only");
    Console.WriteLine("3. Disallow all but one write path");
    Console.WriteLine("4. Disallow all but two write path");
    Console.WriteLine("5. Disallow GETs without select");

    try
    {
        choice = int.Parse(Console.ReadLine() ?? string.Empty);
    }
    catch (System.FormatException)
    {
        // Set to invalid value
        choice = -1;
    }

    switch (choice)
    {
        case 0:
            // Exit the program
            Console.WriteLine("Goodbye...");
            break;
        case 1:
            // Allow all requests
            await CallAllRequests(AuthZPolicy.Base);
            break;
        case 2:
            // Allow users path only
            await CallAllRequests(AuthZPolicy.AllowUsersPath);
            break;
        case 3:
            // Disallow all but one write paths
            await CallAllRequests(AuthZPolicy.DisallowAllButOneWritePath);
            break;
        case 4:
            // Disallow all but two write paths
            await CallAllRequests(AuthZPolicy.DisallowAllButTwoWritePaths);
            break;
        case 5:
            // Disallow all GETs without select
            await CallAllRequests(AuthZPolicy.DisallowGETsWithoutSelect);
            break;
        default:
            Console.WriteLine("Invalid choice! Please try again.");
            break;
    }
}

// </ProgramSnippet>

// <InitializeGraphSnippet>
void InitializeGraph(Settings settings)
{
    GraphHelper.InitializeGraphForUserAuth(settings,
        (info, cancel) =>
        {
            // Display the device code message to
            // the user. This tells them
            // where to go to sign in and provides the
            // code to use.
            Console.WriteLine(info.Message);
            return Task.FromResult(0);
        });
}
// </InitializeGraphSnippet>

// <GreetUserSnippet>
async Task CallAllRequests(AuthZPolicy policy)
{
    // List emails from user's inbox
    await ListInboxAsync(policy);
    // Send an email message
    await SendMailAsync(policy);
    // List users
    await ListUsersAsync(policy);
}

async Task GreetUserAsync(AuthZPolicy policy)
{
    try
    {
        var user = await GraphHelper.GetUserAsync(policy);
        Console.WriteLine($"Hello, {user?.DisplayName}!");
        // For Work/school accounts, email is in Mail property
        // Personal accounts, email is in UserPrincipalName
        Console.WriteLine($"Email: {user?.Mail ?? user?.UserPrincipalName ?? ""}");
    }
    catch (Exception ex)
    {
        WriteError("Error getting user", ex);
    }
}
// </GreetUserSnippet>


// <ListInboxSnippet>
async Task ListInboxAsync(AuthZPolicy policy)
{
    try
    {
        var messagePage = await GraphHelper.GetInboxAsync(policy);

        // Output each message's details
        foreach (var message in messagePage.CurrentPage)
        {
            Console.WriteLine($"Message: {message.Subject ?? "NO SUBJECT"}");
            Console.WriteLine($"  From: {message.From?.EmailAddress?.Name}");
            Console.WriteLine($"  Status: {(message.IsRead!.Value ? "Read" : "Unread")}");
            Console.WriteLine($"  Received: {message.ReceivedDateTime?.ToLocalTime().ToString()}");
        }

        // If NextPageRequest is not null, there are more messages
        // available on the server
        // Access the next page like:
        // messagePage.NextPageRequest.GetAsync();
        var moreAvailable = messagePage.NextPageRequest != null;

        Console.WriteLine($"\nMore messages available? {moreAvailable}");
    }
    catch (Exception ex)
    {
        WriteError("Error getting user's inbox", ex);
    }
}
// </ListInboxSnippet>

// <SendMailSnippet>
async Task SendMailAsync(AuthZPolicy policy)
{
    try
    {
        // Send mail to the signed-in user
        // Get the user for their email address
        var user = await GraphHelper.GetUserAsync(policy);

        var userEmail = user?.Mail ?? user?.UserPrincipalName;

        if (string.IsNullOrEmpty(userEmail))
        {
            Console.WriteLine("Couldn't get your email address, canceling...");
            return;
        }

        await GraphHelper.SendMailAsync("Testing Microsoft Graph",
            "Hello world!", userEmail, policy);

        Console.WriteLine("Mail sent.");
    }
    catch (Exception ex)
    {
        WriteError("Error sending mail", ex);
    }
}
// </SendMailSnippet>

// <ListUsersSnippet>
async Task ListUsersAsync(AuthZPolicy policy)
{
    try
    {
        var userPage = await GraphHelper.GetUsersAsync(policy);

        // Output each users's details
        foreach (var user in userPage.CurrentPage)
        {
            Console.WriteLine($"User: {user.DisplayName ?? "NO NAME"}");
            Console.WriteLine($"  ID: {user.Id}");
            Console.WriteLine($"  Email: {user.Mail ?? "NO EMAIL"}");
        }

        // If NextPageRequest is not null, there are more users
        // available on the server
        // Access the next page like:
        // userPage.NextPageRequest.GetAsync();
        var moreAvailable = userPage.NextPageRequest != null;

        Console.WriteLine($"\nMore users available? {moreAvailable}");
    }
    catch (Exception ex)
    {
        WriteError("Error getting users", ex);
    }
}
// </ListUsersSnippet>
void WriteError(string message, Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"{message}: {ex.InnerException?.Message ?? ex.Message}");
    Console.ResetColor();
}
