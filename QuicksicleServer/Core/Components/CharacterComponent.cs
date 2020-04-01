using System;
using Quicksicle.Enums;
using Quicksicle.IO;
using Quicksicle.Objects;
using Quicksicle.Database;

namespace Quicksicle.Core.Components
{
    public class CharacterComponent : BaseReplicaComponent
    {
        public PossessionControlComponent PossessionControlComponent = new PossessionControlComponent();
        public LevelProgressionComponent LevelProgressionComponent = new LevelProgressionComponent();
        public PlayerForcedMovementComponent PlayerForcedMovementComponent = new PlayerForcedMovementComponent();
        public CharacterInfo CharacterInfo; // Has to be set.
        public AccountInfo AccountInfo; // Has to be set.
        public bool IsLaunching = false;
        public bool IsLanding = false;
        public string LandingRocketTextLdf = String.Empty;
        public bool PvpEnabled = false;
        public GameActivity CurrentActivity = GameActivity.None;

        public CharacterComponent() : base(ReplicaComponentId.Character) { }

        public override void ToBitStream(WriteOnlyBitStream packetStream, bool isInitialUpdate)
        {
            PossessionControlComponent.ToBitStream(packetStream, isInitialUpdate);
            LevelProgressionComponent.ToBitStream(packetStream, isInitialUpdate);
            PlayerForcedMovementComponent.ToBitStream(packetStream, isInitialUpdate);

            if (isInitialUpdate)
            {
                packetStream.Write0();
                packetStream.Write0();
                packetStream.Write0();
                packetStream.Write0();

                packetStream.Write(CharacterInfo.HairColor);
                packetStream.Write(CharacterInfo.HairStyle);
                packetStream.Write(CharacterInfo.Head);
                packetStream.Write(CharacterInfo.ChestColor);
                packetStream.Write(CharacterInfo.Legs);
                packetStream.Write(CharacterInfo.Chest);
                packetStream.Write(CharacterInfo.HeadColor);
                packetStream.Write(CharacterInfo.EyebrowStyle);
                packetStream.Write(CharacterInfo.EyesStyle);
                packetStream.Write(CharacterInfo.MouthStyle);
                packetStream.Write(AccountInfo.AccountId);
                packetStream.Write(CharacterInfo.LastLogout);
                packetStream.Write((ulong)0);
                packetStream.Write(CharacterInfo.UniverseScore);
                packetStream.Write(false);

                // this is only temporary
                for (int i = 0; i < 27; i++)
                {
                    packetStream.Write((ulong)0);
                }

                packetStream.Write(IsLaunching);

                if (IsLanding)
                {
                    packetStream.Write1();
                    packetStream.Write((ushort)LandingRocketTextLdf.Length);
                    packetStream.WriteWideString(LandingRocketTextLdf, LandingRocketTextLdf.Length);
                }
                else
                {
                    packetStream.Write0();
                }
            }

            packetStream.Write1();
            packetStream.Write(PvpEnabled);
            packetStream.Write(CharacterInfo.GmLevel > 0);
            packetStream.Write(CharacterInfo.GmLevel);
            packetStream.Write(CharacterInfo.EditorLevel > 0);
            packetStream.Write(CharacterInfo.EditorLevel);

            packetStream.Write1();
            packetStream.Write((uint)CurrentActivity);

            packetStream.Write0();
        }
    }
}
