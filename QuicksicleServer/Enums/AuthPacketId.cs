using System;

namespace Quicksicle.Enums
{
    public enum AuthPacketId : uint
    {
        MSG_AUTH_LOGIN_REQUEST,
        MSG_AUTH_LOGOUT_REQUEST,
        MSG_AUTH_CREATE_NEW_ACCOUNT_REQUEST,
        MSG_AUTH_LEGOINTERFACE_AUTH_RESPONSE,
        MSG_AUTH_SESSIONKEY_RECEIVED_CONFIRM,
        MSG_AUTH_RUNTIME_CONFIG
    }
}
