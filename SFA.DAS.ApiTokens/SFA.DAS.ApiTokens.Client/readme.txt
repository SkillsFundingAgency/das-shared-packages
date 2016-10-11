SFA.DAS.ApiTokens.Client
========================

Include the following in App_Start\WebApiConfig.cs

    public static void Register(HttpConfiguration config)
    {
		// existig code can be here...

        var apiKeySecret = CloudConfigurationManager.GetSetting("ApiTokenSecret");
        var apiIssuer = CloudConfigurationManager.GetSetting("ApiIssuer");
        var apiAudiences = CloudConfigurationManager.GetSetting("ApiAudiences").Split(' ');

        config.MessageHandlers.Add(new ApiKeyHandler("Authorization", apiKeySecret, apiIssuer, apiAudiences));

		// existig code can be here...
    }

The code relies on 3 config settings for the secret, issuer and audience. 

Further details can be found here https://github.com/SkillsFundingAgency/das-shared-packages/tree/master/SFA.DAS.ApiTokens
