// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Azure.Core;
using Azure.Identity;
using GraphTutorial;
using GraphTutorial.AuthZ;
using GraphTutorial.AuthZ.Models;
using Microsoft.Graph;

class GraphHelper
{
    #region User-auth
    // <UserAuthConfigSnippet>
    // Settings object
    private static Settings? _settings;
    // User auth token credential
    private static DeviceCodeCredential? _deviceCodeCredential;
    // Client configured with user authentication
    private static GraphServiceClient? _userClient;

    public static void InitializeGraphForUserAuth(Settings settings,
        Func<DeviceCodeInfo, CancellationToken, Task> deviceCodePrompt)
    {
        _settings = settings;
        _deviceCodeCredential = new DeviceCodeCredential(deviceCodePrompt, settings.AuthTenant, settings.ClientId);

        var authProvider = new TokenCredentialAuthProvider(_deviceCodeCredential, settings.GraphUserScopes);

        var handlers = GraphClientFactory.CreateDefaultHandlers(authProvider);
        handlers.Insert(1, new AuthZHandler());

        var httpClient = GraphClientFactory.Create(handlers);
        _userClient = new GraphServiceClient(httpClient);
    }
    // </UserAuthConfigSnippet>

    // <GetUserTokenSnippet>
    public static async Task<string> GetUserTokenAsync()
    {
        // Ensure credential isn't null
        _ = _deviceCodeCredential ??
            throw new System.NullReferenceException("Graph has not been initialized for user auth");

        // Ensure scopes isn't null
        _ = _settings?.GraphUserScopes ?? throw new System.ArgumentNullException("Argument 'scopes' cannot be null");

        // Request token with given scopes
        var context = new TokenRequestContext(_settings.GraphUserScopes);
        var response = await _deviceCodeCredential.GetTokenAsync(context);
        return response.Token;
    }
    // </GetUserTokenSnippet>

    // <GetUserSnippet>
    public static Task<User> GetUserAsync(AuthZPolicy policy, bool withSelect = true)
    {
        // Ensure client isn't null
        _ = _userClient ??
            throw new System.NullReferenceException("Graph has not been initialized for user auth");

        var userRequest = _userClient.Me
            .Request()
            .WithAuthZPolicy(policy);

        if (withSelect)
        {
            return userRequest
                .Select(u => new
                {
                    // Only request specific properties
                    u.DisplayName,
                    u.Mail,
                    u.UserPrincipalName
                })
                .GetAsync();
        }
        else
        {
            return userRequest
                .GetAsync();
        }
    }

    // </GetUserSnippet>

    // <GetInboxSnippet>
    public static Task<IMailFolderMessagesCollectionPage> GetInboxAsync(AuthZPolicy policy)
    {
        // Ensure client isn't null
        _ = _userClient ??
            throw new System.NullReferenceException("Graph has not been initialized for user auth");

        return _userClient.Me
            // Only messages from Inbox folder
            .MailFolders["Inbox"]
            .Messages
            .Request()
            .WithAuthZPolicy(policy)
            .Select(m => new
            {
                // Only request specific properties
                m.From,
                m.IsRead,
                m.ReceivedDateTime,
                m.Subject
            })
            // Get at most 2 results
            .Top(2)
            // Sort by received time, newest first
            .OrderBy("ReceivedDateTime DESC")
            .GetAsync();
    }
    // </GetInboxSnippet>

    // <SendMailSnippet>
    public static async Task SendMailAsync(string subject, string body, string recipient, AuthZPolicy policy)
    {
        // Ensure client isn't null
        _ = _userClient ??
            throw new System.NullReferenceException("Graph has not been initialized for user auth");

        // Create a new message
        var message = new Message
        {
            Subject = subject,
            Body = new ItemBody
            {
                Content = body,
                ContentType = BodyType.Text
            },
            ToRecipients = new Recipient[]
            {
                new Recipient
                {
                    EmailAddress = new EmailAddress
                    {
                        Address = recipient
                    }
                }
            }
        };

        // Send the message
        await _userClient.Me
            .SendMail(message)
            .Request()
            .WithAuthZPolicy(policy)
            .PostAsync();
    }
    // </SendMailSnippet>

    // <GetUsersSnippet>
    public static Task<IGraphServiceUsersCollectionPage> GetUsersAsync(AuthZPolicy policy)
    {
        return _userClient.Users
            .Request()
            .WithAuthZPolicy(policy)
            .Select(u => new
            {
                // Only request specific properties
                u.DisplayName,
                u.Id,
                u.Mail
            })
            // Get at most 2 results
            .Top(2)
            // Sort by display name
            .OrderBy("DisplayName")
            .GetAsync();
    }
    // </GetUsersSnippet>
    #endregion
}
