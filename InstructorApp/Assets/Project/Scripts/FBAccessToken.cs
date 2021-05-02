using System;
using System.Collections.Generic;

[Serializable]
public class FBAccessToken
{
    public string TokenString;
    public string ExpirationTime;
    public string LastRefresh;
    public List<string> Permissions;
    public string UserId;
    public string UserType;
}
