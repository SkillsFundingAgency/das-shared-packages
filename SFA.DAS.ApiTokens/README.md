# DAS API token tool (ApiTokenGen)#

Command line tool for creating JWT API tokens for the Digital Apprenticeship Service

Type `ApiTokenGen /?` to view help:

    APITokenGen command line help
    
    This tool generates a JWT for granting access to a single client.
    The generated token is encoded and signed (using the HS256 algorithm). It is NOT encrypted.
    
    The following command line arguments are supported (all case insensitive, any order):
    
      /issuer:<value>    - <value> is the URI of the issuer (the API service)
      /audience:<value>  - <value> is the URI of the audience (the API client/consumer)
      /data:<value>      - <value> is a space delimited list of roles/claims/permissions that the token will contain
      /secret:<value>    - <value> is a phrase used to sign the token (minimum 16 characters)
      /duration:<value>  - <value> is the number of hours until the token expires, defaults to 720 (30 days)
    
    Example:
    
      APITokenGen /issuer:http://server.net /audience:http://client.net /data:"Role1 Role2" /secret:"Some Super Secret" /duration:180
