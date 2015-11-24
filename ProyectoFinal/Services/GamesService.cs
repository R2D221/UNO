using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using EntityFramework.Extensions;
using ProyectoFinal.Models;
using ProyectoFinal.Utils;

namespace ProyectoFinal.Services
{
	public class GamesService : ServiceBase<ApplicationDbContext>
	{
		public GamesService(ApplicationDbContext db = null) : base(db) { }

		private static readonly ConcurrentDictionary<Guid, SessionWithTurns> activeSessions;
		private static readonly ConcurrentDictionary<string, Lazy<ConcurrentDictionary<Guid, SessionWithTurns>>> activeSessionsByUser;
		static GamesService()
		{
			activeSessions = new ConcurrentDictionary<Guid, SessionWithTurns>();
			activeSessionsByUser = new ConcurrentDictionary<string, Lazy<ConcurrentDictionary<Guid, SessionWithTurns>>>();

			using (var db = new ApplicationDbContext())
			{
				//foreach (var card in db.Cards.Where(a => a.Rank == Rank.Wild || a.Rank == Rank.WildDrawFour))
				//{
				//	card.Color = Color.Wild;
				//}
				//db.SaveChanges();
			
				foreach (var session in db.Sessions)
				{
					var sessionWithTurns = new SessionWithTurns(session.Direction, ToModel(session.Hand1), ToModel(session.Hand2), ToModel(session.Hand3), ToModel(session.Hand4))
					{
						LastPlayed = session.LastPlayed,
						Deck = new Stack<Card>(session.Deck.Where(c => !c.IsDiscarded).OrderByDescending(c => c.Order).Select(c => c.Card).ToList()),
						DiscardPile = new Stack<Card>(session.Deck.Where(c => c.IsDiscarded).OrderBy(c => c.Order).Select(c => c.Card).ToList().Concat(new[] { session.DiscardPileTop })),
					};
					activeSessions.TryAdd(session.Id, sessionWithTurns);
					activeSessionsByUser.GetOrAdd(session.Hand1.UserId, _ => new ConcurrentDictionary<Guid, SessionWithTurns>())
						.TryAdd(session.Id, sessionWithTurns);
					activeSessionsByUser.GetOrAdd(session.Hand2.UserId, _ => new ConcurrentDictionary<Guid, SessionWithTurns>())
						.TryAdd(session.Id, sessionWithTurns);
					activeSessionsByUser.GetOrAdd(session.Hand3.UserId, _ => new ConcurrentDictionary<Guid, SessionWithTurns>())
						.TryAdd(session.Id, sessionWithTurns);
					activeSessionsByUser.GetOrAdd(session.Hand4.UserId, _ => new ConcurrentDictionary<Guid, SessionWithTurns>())
						.TryAdd(session.Id, sessionWithTurns);
				}
			}
		}

		private static UserModel ToModel(ApplicationUser user)
		{
			return new UserModel
			{
				Id = user.Id,
				Name = user.Name,
				Photo = user.Photo,
			};
		}

		private static HandModel ToModel(Hand hand)
		{
			return new HandModel
			{
				Id = hand.Id,
				User = ToModel(hand.User),
				IsTheirTurn = hand.IsTheirTurn,
				Cards = hand.Cards.ToList(),
			};
		}

		public Guid Create(string userId1, string userId2, string userId3, string userId4)
		{
			var cardsList = db.Cards.ToList();
			cardsList.Shuffle();
			var cards = new Stack<Card>(cardsList);

			var hand1 = new Hand
			{
				IsTheirTurn = true,
				UserId = userId1,
				Cards = Enumerable.Range(0, 7).Select(_ => cards.Pop()).ToList(),
			};
			var hand2 = new Hand
			{
				IsTheirTurn = false,
				UserId = userId2,
				Cards = Enumerable.Range(0, 7).Select(_ => cards.Pop()).ToList(),
			};
			var hand3 = new Hand
			{
				IsTheirTurn = false,
				UserId = userId3,
				Cards = Enumerable.Range(0, 7).Select(_ => cards.Pop()).ToList(),
			};
			var hand4 = new Hand
			{
				IsTheirTurn = false,
				UserId = userId4,
				Cards = Enumerable.Range(0, 7).Select(_ => cards.Pop()).ToList(),
			};

			var firstCard = cards.Pop();

			var deck = cards.Select((c, i) => new CardInPile
			{
				Card = c,
				IsDiscarded = false,
				Order = i,
			}).ToList();

			var session = new Session
			{
				Direction = Direction.Counterclockwise,
				DiscardPileTop = firstCard,
				Deck = deck,
				LastPlayed = DateTime.UtcNow,
			};

			db.Sessions.Add(session);

			db.SaveChanges();

			hand1.Session = session; session.Hand1 = hand1;
			hand2.Session = session; session.Hand2 = hand2;
			hand3.Session = session; session.Hand3 = hand3;
			hand4.Session = session; session.Hand4 = hand4;

			db.SaveChanges();

			db.Entry(hand1).Reference(a => a.User).Load();
			db.Entry(hand2).Reference(a => a.User).Load();
			db.Entry(hand3).Reference(a => a.User).Load();
			db.Entry(hand4).Reference(a => a.User).Load();

			var sessionWithTurns = new SessionWithTurns(session.Direction, ToModel(hand1), ToModel(hand2), ToModel(hand3), ToModel(hand4))
			{
				LastPlayed = session.LastPlayed,
				Deck = cards,
				DiscardPile = new Stack<Card>(new[] { firstCard }),
			};

			activeSessions.TryAdd(session.Id, sessionWithTurns);
			activeSessionsByUser.GetOrAdd(userId1, _ => new ConcurrentDictionary<Guid, SessionWithTurns>())
				.TryAdd(session.Id, sessionWithTurns);
			activeSessionsByUser.GetOrAdd(userId2, _ => new ConcurrentDictionary<Guid, SessionWithTurns>())
				.TryAdd(session.Id, sessionWithTurns);
			activeSessionsByUser.GetOrAdd(userId3, _ => new ConcurrentDictionary<Guid, SessionWithTurns>())
				.TryAdd(session.Id, sessionWithTurns);
			activeSessionsByUser.GetOrAdd(userId4, _ => new ConcurrentDictionary<Guid, SessionWithTurns>())
				.TryAdd(session.Id, sessionWithTurns);

			return session.Id;
		}

		public IEnumerable<SessionInfoModel> GetActiveSessionsForUser(string userId)
		{
			foreach (var entry in activeSessionsByUser.GetOrAdd(userId, _ => new ConcurrentDictionary<Guid, SessionWithTurns>()).OrderBy(x => x.Value.LastPlayed))
			{
				yield return new SessionInfoModel
				{
					Id = entry.Key,
					Players = entry.Value.Users,
					DiscardPileTop = entry.Value.DiscardPile.Peek(),
				};
			}
		}

		public SessionModel GetSession(Guid sessionId, string userId)
		{
			var session = activeSessions[sessionId];

			return new SessionModel
			{
				Id = sessionId,
				Direction = session.Direction,
				DiscardPileTop = session.DiscardPile.Peek(),
				DeckCount = session.Deck.Count,
				DiscardPileCount = session.DiscardPile.Count - 1,
				Hands = session.HandsVisibleToUser(userId).ToList(),
			};
		}

		public GameUpdateModel TryUseCard(Guid sessionId, string userId, int cardId, Color color)
		{
			var session = activeSessions[sessionId];
			var hand = session.GetHandForUserId(userId);

			if (!hand.IsTheirTurn)
				return null;

			var card = hand.Cards.SingleOrDefault(c => c.Id == cardId);
			if (card == null)
				return null;

			var discardPileTop = session.DiscardPile.Peek();
		
			if (card.Rank == Rank.Wild || card.Rank == Rank.WildDrawFour) //<-- I play a wild card, so it's OK
			{
				//continue
			}
			else if (discardPileTop.Rank == Rank.Wild || discardPileTop.Rank == Rank.WildDrawFour) //<-- There's a wild card on the pile, check which color it is
			{
				if (card.Color != session.Color)
				{
					return null;
				}
			}
			else if (card.Color != discardPileTop.Color && card.Rank != discardPileTop.Rank) //<-- Normal cards, don't match
			{
				return null;
			}

			var dbSession = db.Sessions.Find(sessionId);
			var dbDiscard = dbSession.DiscardPileTop;

			// Move card from hand to discard pile
			{
				hand.Cards.Remove(card);
				session.DiscardPile.Push(card);

				var cardToRemove = db.Cards.Find(card.Id);
				db.Hands.Find(hand.Id).Cards.Remove(cardToRemove);
				var lastOrder = dbSession.Deck.Where(c => c.IsDiscarded).OrderByDescending(c => c.Order).Select(c => c.Order).FirstOrDefault();
				dbSession.Deck.Add(new CardInPile
				{
					Card = dbDiscard,
					IsDiscarded = true,
					Order = lastOrder + 1,
				});
				dbSession.DiscardPileTopId = card.Id;
			}

			ActionModel action = null;

			switch (card.Rank)
			{
				case Rank.Reverse:
				{
					session.Reverse();
				}
				break;
				case Rank.Skip:
				{
					var skippedUserId = session.MoveNext();
					action = new ActionModel
					{
						Rank = card.Rank,
						UserId = skippedUserId,
					};
				}
				break;
				case Rank.DrawTwo:
				{
					var skippedUserId = session.MoveNext();
					List<Card> cardsReceived;
					if (card.Rank == Rank.DrawTwo)
					{
						cardsReceived = new List<Card> { session.Deck.Pop(), session.Deck.Pop() };
					}
					else //if (card.Rank == Rank.WildDrawFour)
					{
						cardsReceived = new List<Card> { session.Deck.Pop(), session.Deck.Pop(), session.Deck.Pop(), session.Deck.Pop() };
					}
					session.GetHandForUserId(skippedUserId).Cards.AddRange(cardsReceived);
					action = new ActionModel
					{
						Rank = card.Rank,
						UserId = skippedUserId,
						CardsReceived = cardsReceived,
					};
				
					var dbHand = db.Hands.Single(a => a.SessionId == sessionId && a.UserId == skippedUserId);
					foreach (var cardReceived in cardsReceived)
					{ 
						var dbCard = db.Cards.Find(cardReceived.Id);
						dbSession.Deck.Remove(dbSession.Deck.Single(a => a.CardId == cardReceived.Id));
						dbHand.Cards.Add(dbCard);
					}
				}
				break;
				case Rank.Wild:
				{
					session.Color = color;
					dbSession.Color = color;
					card = new Card { Id = card.Id, Rank = card.Rank, Color = color };
				}
				break;
				case Rank.WildDrawFour:
				{
					session.Color = color;
					dbSession.Color = color;
					card = new Card { Id = card.Id, Rank = card.Rank, Color = color };
				}
				goto case Rank.DrawTwo;
			}

			// Turn to next user
			var nextUserId = session.MoveNext();

			dbSession.Hand1.IsTheirTurn = dbSession.Hand1.UserId == nextUserId;
			dbSession.Hand2.IsTheirTurn = dbSession.Hand2.UserId == nextUserId;
			dbSession.Hand3.IsTheirTurn = dbSession.Hand3.UserId == nextUserId;
			dbSession.Hand4.IsTheirTurn = dbSession.Hand4.UserId == nextUserId;
		
			dbSession.Direction = session.Direction;
		
			dbSession.LastPlayed = DateTime.UtcNow;
			session.LastPlayed = dbSession.LastPlayed;

			db.SaveChanges();

			return new GameUpdateModel
			{
				Direction	= session.Direction,
				Card	= card,
				PreviousUserId	= userId,
				NextUserId	= nextUserId,
				Action	= action,
			};
		}

		public GameUpdateModel TryDrawCard(Guid sessionId, string userId)
		{
			var session = activeSessions[sessionId];
			var hand = session.GetHandForUserId(userId);
			var dbSession = db.Sessions.Find(sessionId);

			if (!hand.IsTheirTurn)
				return null;
		
			var card = session.Deck.Pop();
			session.GetHandForUserId(userId).Cards.Add(card);
				
			var dbHand = db.Hands.Single(a => a.SessionId == sessionId && a.UserId == userId);
			var dbCard = db.Cards.Find(card.Id);
			dbSession.Deck.Remove(dbSession.Deck.Single(a => a.CardId == card.Id));
			dbHand.Cards.Add(dbCard);
		
			var nextUserId = session.MoveNext();

			dbSession.Hand1.IsTheirTurn = dbSession.Hand1.UserId == nextUserId;
			dbSession.Hand2.IsTheirTurn = dbSession.Hand2.UserId == nextUserId;
			dbSession.Hand3.IsTheirTurn = dbSession.Hand3.UserId == nextUserId;
			dbSession.Hand4.IsTheirTurn = dbSession.Hand4.UserId == nextUserId;
		
			dbSession.LastPlayed = DateTime.UtcNow;
			session.LastPlayed = dbSession.LastPlayed;

			db.SaveChanges();

			return new GameUpdateModel
			{
				Card	= card,
				PreviousUserId	= userId,
				NextUserId	= nextUserId,
			};
		}
	}
}