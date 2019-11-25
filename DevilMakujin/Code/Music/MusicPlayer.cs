using DevilMakujin.Code.Entity;
using DevilMakujin.Properties;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using static DevilMakujin.Code.GlobalDrawArranger;
using static DevilMakujin.Code.StageManager;

namespace DevilMakujin.Code.Music
{
    public enum SoundEvent
    {
        GameStart,
        NearingVoid,
        MapBossWarning,
        StageClear,

        PlayerDeath,
        PlayerDamage,
        ShieldGone,
        ShieldUp,
    }

    public enum ImmediateSoundEvent
    {
        GenericEnemyDeath,
        SmolWhaleKoDeath,
        BigWhaleKoDeath,
        BulletHit
    }

    public enum BossSoundEvent
    {
        Death,
    }

    public class MusicPlayer
    {
        private readonly Dictionary<Stage, Song> stageMusic;
        private readonly Dictionary<ScreenState, Song> otherMusic;

        private readonly Dictionary<SoundEvent, SoundEffectInstance> otherSound; //maybe play music like huge soundeffects?
        private readonly Dictionary<Bullet.BulletType, SoundEffect> gunSound;
        private readonly Dictionary<ImmediateSoundEvent, SoundEffect> overlappableOtherSound;
        private readonly Dictionary<BossSoundEvent,SoundEffectInstance> eventBossPhrases;

        private readonly List<SoundEffectInstance> randomBossPhrases;

        private Stage oldStage = Stage.None;

        public MusicPlayer(DevimakuGame game)
        {
            this.stageMusic = new Dictionary<Stage, Song>
            {
                { Stage.OuterRing, game.Content.Load<Song>("music/earlygamesec") },
                { Stage.MediumRing, game.Content.Load<Song>("music/midgamesec") },
                { Stage.InnerRing, game.Content.Load<Song>("music/lategamesec") },
                { Stage.Center, game.Content.Load<Song>("music/ginga_no_chuushin_no_kaibutsu") }
            };

            this.otherMusic = new Dictionary<ScreenState, Song>
            {
                { ScreenState.Map, game.Content.Load<Song>("music/mapgamesec") },
                { ScreenState.EndGame, game.Content.Load<Song>("music/theusualcreditsgame") },
            };

            this.otherSound = new Dictionary<SoundEvent, SoundEffectInstance>
            {
                { SoundEvent.GameStart, game.Content.Load<SoundEffect>("sound/game start 2").CreateInstance() },
                { SoundEvent.NearingVoid, game.Content.Load<SoundEffect>("sound/oobwarning").CreateInstance() },
                { SoundEvent.MapBossWarning, game.Content.Load<SoundEffect>("sound/bosswarning").CreateInstance() },
                { SoundEvent.StageClear, game.Content.Load<SoundEffect>("sound/stageclear").CreateInstance() },

                { SoundEvent.ShieldUp, game.Content.Load<SoundEffect>("sound/shieldup").CreateInstance() },
                { SoundEvent.ShieldGone, game.Content.Load<SoundEffect>("sound/shieldgone").CreateInstance() },
                { SoundEvent.PlayerDeath, game.Content.Load<SoundEffect>("sound/death").CreateInstance() },
                { SoundEvent.PlayerDamage, game.Content.Load<SoundEffect>("sound/runnerhit").CreateInstance() },
            };

            this.eventBossPhrases = new Dictionary<BossSoundEvent, SoundEffectInstance>
            {
                { BossSoundEvent.Death, game.Content.Load<SoundEffect>("sound/whalery/wsdeath").CreateInstance() },
            };

            this.gunSound = new Dictionary<Bullet.BulletType, SoundEffect>
            {
                { Bullet.BulletType.Blaster, game.Content.Load<SoundEffect>("sound/newlonglazer") },
                { Bullet.BulletType.SpeedBlaster, game.Content.Load<SoundEffect>("sound/newmachinegun") },
                { Bullet.BulletType.LongBlaster, game.Content.Load<SoundEffect>("sound/newnormalshot") },
            };

            this.overlappableOtherSound = new Dictionary<ImmediateSoundEvent, SoundEffect>
            {
                { ImmediateSoundEvent.GenericEnemyDeath, game.Content.Load<SoundEffect>("sound/enemy_kill3") },
                { ImmediateSoundEvent.SmolWhaleKoDeath, game.Content.Load<SoundEffect>("sound/smolwhalekokill") },
                { ImmediateSoundEvent.BigWhaleKoDeath, game.Content.Load<SoundEffect>("sound/whale-chan kill") },
                { ImmediateSoundEvent.BulletHit, game.Content.Load<SoundEffect>("sound/enemy_kill3") },
            };

            const string folder = "sound/whalery/";
            this.randomBossPhrases = new List<SoundEffectInstance>
            {
                game.Content.Load<SoundEffect>(folder + "whale 1 rev2").CreateInstance(),
                game.Content.Load<SoundEffect>(folder + "whale destroy").CreateInstance(),
                game.Content.Load<SoundEffect>(folder + "whale fear me 4").CreateInstance(),
                game.Content.Load<SoundEffect>(folder + "whale i can see you 2").CreateInstance(),
                game.Content.Load<SoundEffect>(folder + "whale i will devour you 3").CreateInstance(),
                game.Content.Load<SoundEffect>(folder + "whale laugh").CreateInstance(),
                game.Content.Load<SoundEffect>(folder + "whale nowhere to run").CreateInstance(),
                game.Content.Load<SoundEffect>(folder + "whale there is no escape 2").CreateInstance(),
                game.Content.Load<SoundEffect>(folder + "whale you cannot escape your fate").CreateInstance(),
            };

            MediaPlayer.IsRepeating = true;
        }

        /// <summary>
        /// Is called every stage
        /// </summary>
        public void InitStageLoopIfPossible(Stage stageSong, bool forceRestart = false)
        {
            if ((stageSong != oldStage || forceRestart) && stageMusic.ContainsKey(stageSong))
            {
                MediaPlayer.Stop();
                MediaPlayer.Volume = Settings.Default.StageMusicVolume; //todo
                MediaPlayer.Play(stageMusic[CurrentStage]);
            }

            if (stageSong == Stage.None)
                MediaPlayer.Stop();

            oldStage = stageSong;
        }

        /// <summary>
        /// Is not called every stage, but on demand
        /// </summary>
        /// <param name="state"></param>
        public void InitOtherLoopIfPossible(ScreenState state)
        {
            if (otherMusic.ContainsKey(state))
            {
                MediaPlayer.Stop();
                MediaPlayer.Volume = Settings.Default.OtherMusicVolume;
                MediaPlayer.Play(otherMusic[state]);
            }
        }

        public void PlaySound(SoundEvent sound)
        {
            if (this.otherSound.ContainsKey(sound))
            {
                this.otherSound[sound].Volume = Settings.Default.SoundVolume;
                this.otherSound[sound].Play();
            }
        }

        public void PlaySound(Bullet.BulletType sound)
        {
            if (this.gunSound.ContainsKey(sound))
                this.gunSound[sound].Play(Settings.Default.GunVolume, 0.0f, 0.0f);
        }

        public void PlayRandomBossPhrase()
        {
            int rand = DevimakuGame.Rand.Next(randomBossPhrases.Count);
            this.randomBossPhrases[rand].Volume = Settings.Default.VoiceVolume;
            this.randomBossPhrases[rand].Play();
        }

        public void PlayBossPhraseFate()
        {
            int i = randomBossPhrases.Count - 1;
            this.randomBossPhrases[i].Volume = Settings.Default.VoiceVolume;
            this.randomBossPhrases[i].Play();
        }

        internal void PlayBossEventPhrase(BossSoundEvent sound)
        {
            if(eventBossPhrases.ContainsKey(sound))
            {
                eventBossPhrases[sound].Volume = Settings.Default.VoiceVolume;
                eventBossPhrases[sound].Play();
            }
        }

        internal void PlayEventOnEntityDeath(AEntity entity)
        {
            float volume = Settings.Default.GunVolume;

            if (entity is BossEntity)
            {
                this.PlayBossEventPhrase(BossSoundEvent.Death);
            }
            else if (entity is BossLeech)
            {
                bool isBig = ((BossLeech)entity).isBig;
                if(isBig)
                    this.overlappableOtherSound[ImmediateSoundEvent.BigWhaleKoDeath].Play(volume, 0.0f, 0.0f);
                else
                    this.overlappableOtherSound[ImmediateSoundEvent.SmolWhaleKoDeath].Play(volume, 0.0f, 0.0f);
            }
            else if (entity is GenericEnemy)
            {
                this.overlappableOtherSound[ImmediateSoundEvent.GenericEnemyDeath].Play(volume, 0.0f, 0.0f);
            }
            else if (entity is Bullet)
            {
                return;
            }
        }
    }
}
