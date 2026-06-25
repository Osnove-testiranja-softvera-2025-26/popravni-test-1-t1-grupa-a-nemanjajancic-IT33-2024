using NUnit.Framework;
using OTS2026_PT1_GrupaA.Exceptions;
using OTS2026_PT1_GrupaA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTS2026_PT1_GrupaA.Test
{
    public class GameTest
    {
        [TestFixture]
        public class GameTests
        {
            [TestCase(20, 0, "Igrač u Pond zoni")]
            [TestCase(0, 0, "Igrač u Invalid zoni")]
            public void False_ValidateLocationInsideMapForPlayer_ThrowsException(int x, int y, string description)
            {
                Position player = new Position(x, y);
                Position boat = new Position(0, 5);

                InvalidPlayerPositionException exception = Assert.Throws<InvalidPlayerPositionException>((TestDelegate)(() => new Game(player, boat)));

                Assert.That(exception.Message, Is.EqualTo("Player and boat must be in the Land zone!"));
            }

            [TestCase(20, 0, "Čamac u Pond zoni")]
            [TestCase(0, 0, "Čamac u Invalid zoni")]
            public void False_ValidateLocationInsideMapForBoat_ThrowsException(int x, int y, string description)
            {
                Position player = new Position(0, 5);
                Position boat = new Position(x, y);

                InvalidPlayerPositionException exception = Assert.Throws<InvalidPlayerPositionException>((TestDelegate)(() => new Game(player, boat)));

                Assert.That(exception.Message, Is.EqualTo("Player and boat must be in the Land zone!"));
            }


            [Test]
            public void SpawnPlayerAndBoat_Successfully()
            {
                Position player = new Position(0, 5);
                Position boat = new Position(0, 6);

                Game game = new Game(player, boat);

                Assert.That(game.Player.Position, Is.EqualTo(player));
            }


            [Test]
            public void MovePlayerUp_Successfully()
            {
                Position player = new Position(5, 15);
                Position boat = new Position(5, 16);

                Game game = new Game(player, boat);

                game.MovePlayer(Move.Up);

                Assert.That(game.Player.Position.Y, Is.EqualTo(14));
            }
            [Test]
            public void MovePlayerDown_Successfully()
            {
                Position player = new Position(5, 15);
                Position boat = new Position(5, 16);

                Game game = new Game(player, boat);

                game.MovePlayer(Move.Down);

                Assert.That(game.Player.Position.Y, Is.EqualTo(16));
            }

            [Test]
            public void MovePlayerLeft_Successfully()
            {
                Position player = new Position(5, 15);
                Position boat = new Position(5, 16);

                Game game = new Game(player, boat);

                game.MovePlayer(Move.Left);

                Assert.That(game.Player.Position.X, Is.EqualTo(4));
            }

            [Test]
            public void MovePlayerRight_Successfully()
            {
                Position player = new Position(5, 15);
                Position boat = new Position(5, 16);

                Game game = new Game(player, boat);

                game.MovePlayer(Move.Right);

                Assert.That(game.Player.Position.X, Is.EqualTo(6));
            }

            [Test]
            public void MovePlayerOutsideOfBoundaries()
            {
                Position player = new Position(0, 5);
                Position boat = new Position(0, 6);

                Game game = new Game(player, boat);

                game.MovePlayer(Move.Left);

                Assert.That(game.Player.Position, Is.EqualTo(player));
            }

            [Test]
            public void MovePlayerIntoTheInvalidZone()
            {
                Position player = new Position(0, 5);
                Position boat = new Position(0, 6);

                Game game = new Game(player, boat);

                game.MovePlayer(Move.Up);

                Assert.That(game.Player.Position, Is.EqualTo(player));
            }

            [Test]
            public void MovePlayerIntoPond_WithoutBoat_Fails()
            {
                Position player = new Position(19, 15);
                Position boat = new Position(0, 5);

                Game game = new Game(player, boat);
                game.Player.HasBoat = false;

                game.MovePlayer(Move.Right);

                Assert.That(game.Player.Position, Is.EqualTo(player));
            }

            [Test]
            public void PlayerGatheringResources_Bait()
            {
                Position player = new Position(5, 15);
                Position boat = new Position(5, 16);

                Game game = new Game(player, boat);

                Position targetPos = new Position(5, 16);
                game.Map.AddContentToFieldOnPosition(FieldContent.Bait, targetPos);

                game.MovePlayer(Move.Down);
                game.ResolvePlayerPosition();

                Assert.That(game.Player.AmountOfBait, Is.EqualTo(1));
                Assert.That(game.Map.Fields[5, 16].Content, Is.EqualTo(FieldContent.Empty));
            }


            [Test]
            public void PlayerGatheringResources_Boat()
            {
                Position player = new Position(5, 15);
                Position boat = new Position(5, 16);

                Game game = new Game(player, boat);

                Position targetPos = new Position(5, 16);
                game.Map.AddContentToFieldOnPosition(FieldContent.Boat, targetPos);

                game.MovePlayer(Move.Down);
                game.ResolvePlayerPosition();

                Assert.That(game.Player.HasBoat, Is.EqualTo(true));
                Assert.That(game.Map.Fields[5, 16].Content, Is.EqualTo(FieldContent.Empty));
            }

            [Test]
            public void PlayerGatheringResources_FishWithBait()
            {
                Position player = new Position(19, 15);
                Position boat = new Position(0, 5);

                Game game = new Game(player, boat);
                game.Player.HasBoat = true;
                game.Player.AmountOfBait = 1;

                Position pondPos = new Position(20, 15);
                game.Map.AddContentToFieldOnPosition(FieldContent.Fish, pondPos);

                game.MovePlayer(Move.Right);
                game.ResolvePlayerPosition();

                Assert.That(game.Player.AmountOfFish, Is.EqualTo(1));
                Assert.That(game.Player.AmountOfBait, Is.EqualTo(0));
                Assert.That(game.Map.Fields[20, 15].Content, Is.EqualTo(FieldContent.Empty));
            }

            [Test]
            public void PlayerGatheringResources_FishNoBait()
            {
                Position player = new Position(19, 15);
                Position boat = new Position(0, 5);

                Game game = new Game(player, boat);
                game.Player.HasBoat = true;
                game.Player.AmountOfBait = 0;

                Position pondPos = new Position(20, 15);
                game.Map.AddContentToFieldOnPosition(FieldContent.Fish, pondPos);

                game.MovePlayer(Move.Right);
                game.ResolvePlayerPosition();

                Assert.That(game.Player.AmountOfFish, Is.EqualTo(0));
                Assert.That(game.Map.Fields[20, 15].Content, Is.EqualTo(FieldContent.Empty));
            }

            [TestCase(13, 0, false, Game.Score.Good)]
            [TestCase(12, 10, true, Game.Score.Average)]
            [TestCase(7, 10, true, Game.Score.Good)]
            [TestCase(6, 10, true, Game.Score.Average)]
            [TestCase(7, 9, true, Game.Score.Bad)]
            [TestCase(0, 0, false, Game.Score.Bad)]
            public void Calculate(int amountOfFish, int amountOfBait, bool hasBoat, Game.Score expected)
            {
                Position player = new Position(0, 5);
                Position boat = new Position(0, 6);

                Game game = new Game(player, boat);
                game.Player.AmountOfFish = amountOfFish;
                game.Player.AmountOfBait = amountOfBait;
                game.Player.HasBoat = hasBoat;

                Game.Score s = game.CalculateIncome();

                Assert.That(s, Is.EqualTo(expected));

            }
        }
    }
}
