
namespace tacs.Structures
{
    public enum ClientState : byte
    {
        Disconnected = 0,
        Connecting,
        Connected,
        LobbyIdle,
        LobbyIgnore,
        LobbyReady,
        InGame
    }

    public enum TileState : byte
    {
        UnMarked = 0,
        PlayerOne,
        PlayerTwo
    }
}
