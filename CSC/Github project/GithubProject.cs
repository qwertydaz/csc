using System;
using System.Collections.Generic;

namespace GithubProject {
	public abstract class Card
	{
		private readonly string name;
		private readonly string type;
		private readonly string ability;

		/*
		 * Card(name, type, ability)
		 * name - the name of the card
		 * type - the type of the card
		 * ability - what the card can do
		 */
		public Card(string name, string type, string ability)
		{
			this.name = name;
			this.type = type;
			this.ability = ability;
		}

		/*
		 * returns name of card
		 * GetCardName()
		 */
		public string GetCardName()
		{
			return name;
		}


		/*
		 * returns type of card
		 * GetCardType()
		 */
		public string GetCardType()
		{
			return type;
		}

		/*
		 * returns name and type of a card object in one string
		 * GetCardDetails()
		 */
		public string GetCardDetails()
		{
			string details = "[";
			if (this.name != null)
			{
				details += this.name + ", ";
			}
			else
			{
				details += "default_name, ";
			}

			if (this.type != null)
			{
				details += this.type;
			}
			else
			{
				details += "default_type";
			}

			return details += "]";
		}

	}

	public abstract class CardSequence
	{
		/*
		 * ExchangeSequence(subSequence, superSequence, len)
		 * subSequence - list that will be expanded
		 * superSequence - list that will be extracted from
		 * len - num of elements in superSequence that will be moved into subSequence (null means all elements in superSequence)
		 */
		public List<Card> ExchangeSequence(int len, List<Card> subSequence, List<Card> superSequence)
		{
			if (len < 1)
			{
				len = superSequence.Count;
			}

			for (int i = 0; i < len; ++i)
			{
				subSequence.Add(superSequence[i]);
			}

			for (int i = 0; i < len; ++i)
			{
				superSequence.Remove(subSequence[i]);
			}

			return subSequence;
		}

		public List<Card> ExchangeSequence(List<Card> subSequence, List<Card> superSequence)
		{
			foreach (Card card in superSequence)
			{
				subSequence.Add(card);
			}

			foreach (Card card in subSequence)
			{
				superSequence.Remove(card);
			}

			return subSequence;
		}

		public static string GetSequenceDetails(int len, List<Card> cardSequence)
		{
			if (len < 1)
			{
				return "[]";
			}

			string sequenceString = "[";
			int count = 0;
			for (int i = cardSequence.Count-1; i > len; i--)
			{
				sequenceString += cardSequence[i].GetCardDetails();
				if (++count != cardSequence.Count)
				{
					sequenceString += ", ";
				}

			}

			return sequenceString += "]";
		}

		/*
		 * Method for converting a list of card objects into a string
		 * GetSequenceDetails(cardSequence)
		 * cardSequence - a card sequence to be converted into a string
		 */
		public static string GetSequenceDetails(List<Card> cardSequence)
		{
			string sequenceString = "[";
			int count = 0;
			foreach (Card card in cardSequence)
			{
				sequenceString += card.GetCardDetails();
				if (++count != cardSequence.Count)
				{
					sequenceString += ", ";
				}

			}

			return sequenceString += "]";
		}
		
		/*
		 * randomises the order of a list
		 * Shuffle(list)
		 * list - list to be shuffled
		 */
		public void Shuffle<Card>(List<Card> list) // fisher-yates shuffle
		{
			Random random = new Random();
			int n = list.Count;
			for (int i = 0; i < (n - 1); i++)
			{
				int randInt = i + random.Next(n - i);
				Card temp = list[randInt];
				list[randInt] = list[i];
				list[i] = temp;
			}

		}

		public Boolean IsEmpty(List<Card> cardSequence)
		{
			if (cardSequence.Count == 0)
			{
				return true;
			}

			return false;
		}

		public List<Card> GetCardList(Card card)
		{
			List<Card> cardList = new List<Card>
			{
				card
			};

			return cardList;
		}

	}

	public class ColourCard : Card
	{
		/*
		 * ColourCard(card_type, colour)
		 * card_type - the name of the card
		 * colour - the colour of the card
		 * ability - to be implemented
		 */
		public ColourCard(string card_type, string colour) : base(card_type, colour, null)
		{
			//TODO ability
		}

	}

	public class Deck : CardSequence
	{
		private readonly string[] colours = new string[]
		{
			"red", "blue", "yellow", "green"
		};

		private readonly string[] card_types = new string[]
		{
			"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "draw_2", "reverse", "skip", "wild", "wild_draw_4"
		};

		private readonly List<Card> population = new List<Card>();

		/*
		 * this initialises all available Card objects and adds them to a list
		 * GenStartDeck()
		 * population - list of all unassigned Card objects
		 */
		public void GenStartDeck()
		{
			foreach (string card_type in card_types)
			{
				foreach (string colour in colours)
				{
					if (card_type != "wild" && card_type != "wild_draw_4")
					{
						Card colourCard = new ColourCard(card_type, colour);
						population.Add(colourCard);
						if (card_type != "0")
						{
							Card colourCard2 = new ColourCard(card_type, colour);
							population.Add(colourCard2);
						}

					}
					else
					{
						Card wildCard = new WildCard(card_type);
						population.Add(wildCard);
					}

				}

			}

			//Shuffles the list to randomise each game (fisher-yates shuffle)
			base.Shuffle(population);
		}

		/*
		 * returns the list of unassigned Card objects
		 * GetStartDeck()
		 */
		public List<Card> GetStartDeck()
		{
			return population;
		}

	}

	public class DiscardPile: Deck
	{
		private List<Card> discardPile = new List<Card>();

		public DiscardPile(Deck remainingDeck)
		{
			ExchangeSequence(1, discardPile, remainingDeck.GetStartDeck());
		}

		public List<Card> GetDiscardPileList()
		{
			return discardPile;
		}

		public void SetDiscardPileList(List<Card> cardList)
		{
			this.discardPile = cardList;
		}

	}

	public class Hand: Deck
	{
		private List<Card> handSequence = new List<Card>();

		/*
		 * removes 7 cards from the unassigned card sequence and adds them to handSequence
		 * Hand(remainingDeck)
		 * remainingDeck - remaining cards left to assign to card sequences (7 will be added to handSequence)
		 */
		public Hand(Deck remainingDeck)
		{
			ExchangeSequence(7, handSequence, remainingDeck.GetStartDeck());
		}

		/*
		 * returns list of cards in Hand
		 * GetHandList()
		 */
		public List<Card> GetHandList()
		{
			return handSequence;
		}

		/*
		 * replaces current card sequence with another
		 * SetHandList(cardList)
		 * cardList - list of cards to replace Hand
		 */
		public void SetHandList(List<Card> cardList)
		{
			this.handSequence = cardList;
		}
		
	}

	public abstract class Person
	{
		private readonly string name;

		/*
		 * Person(name, cardSequence)
		 * name - the name of the person
		 */
		public Person(string name)
		{
			this.name = name;
		}

		/*
		 * returns the name of the person
		 * GetName()
		 */
		public string GetName()
		{
			return name;
		}

	}

	public class PickupPile: Deck
	{
		private readonly List<Card> pickupPile = new List<Card>();

		public PickupPile(Deck remainingDeck)
		{
			ExchangeSequence(pickupPile, remainingDeck.GetStartDeck());
		}

	}

	public class Player: Person
	{
		private readonly Hand hand;

		/*
		 * Player(name, hand)
		 * name - the name of the player
		 * hand - the card sequence assigned to the player
		 */
		public Player(string name, Hand hand): base(name)
		{ 
			this.hand = hand;
		}

		/*
		 * returns hand object (sequence of cards)
		 * GetHand()
		 */
		public Hand GetHand()
		{
			return hand;
		}

	}

	public class UnoGame
	{
		public static void Main(string[] args)
		{
			while (true)
			{
				Console.WriteLine("\nWelcome to Uno v4" +
						"\n\t1. New Game" +
						"\n\t2. Exit");
				string menuInput = Read("choice");
				if (menuInput.Equals("1"))
				{
					int numOfPlayers = ProbeNumOfPlayers();
					Deck startDeck = new Deck();
					startDeck.GenStartDeck();
					List<Player> playerList = new List<Player>();
					while (true)
					{
						for (int i = 1; i < numOfPlayers + 1; ++i)
						{
							Console.WriteLine("\nPlayer NO. " + i);
							string playerName = Read("name");
							Hand hand = new Hand(startDeck);
							Player newPlayer = new Player(playerName, hand);
							playerList.Add(newPlayer);
						}

						DiscardPile discard = new DiscardPile(startDeck);
						PickupPile pickup = new PickupPile(startDeck);

						int currentPlayer = 0;
						while (true)
						{
							Hand hand = playerList[currentPlayer].GetHand();
							List<Card> handList = hand.GetHandList();
							List<Card> discardList = discard.GetDiscardPileList();
							Console.WriteLine("\nCurrent Player: " + playerList[currentPlayer].GetName() +
								"\nYour Current Hand:\n" + Hand.GetSequenceDetails(playerList[currentPlayer].GetHand().GetHandList()) +
								"\nThe top card on the Discard Pile:\n" + DiscardPile.GetSequenceDetails(1, discard.GetDiscardPileList()) +
								"\n\nGame Options");
							if (IsTurnValid(discardList[discardList.Count - 1], handList))
							{
								Console.WriteLine("\t1. Place a Card onto Discard Pile" +
									"\n\t2. Quit Current Game");
								string playerInput = Read("choice");
								if (playerInput.Equals("1"))
								{
									while (true)
									{
										int upperBound = hand.GetHandList().Count - 1;
										Console.WriteLine(Hand.GetSequenceDetails(handList));
										int cardInput = int.Parse(Read("choice (index from 0-" + upperBound + ")"));
										if (cardInput < 0 || cardInput > upperBound)
										{
											Console.WriteLine("\nOut of Range (must be between 0 and " + upperBound + " inclusive)");
										}
										else if (IsCardSimilar(handList[cardInput], discardList[discardList.Count - 1]))
										{
											discard.ExchangeSequence(discardList, hand.GetCardList(handList[cardInput]));
											break;
										}
										else
										{
											Console.WriteLine("\nThis card cannot be placed");
										}

									}

									break;
								}
								else if (playerInput.Equals("2"))
								{
									break;
								}
								else
								{
									Console.WriteLine("\nInvalid Choice");
								}

							}
							else
							{
								Console.WriteLine("You do not have any placeable Cards" +
									"\nYou picked up x Cards from the Pickup Pile and finished your turn"); //fix x
								if (currentPlayer == numOfPlayers - 1)
								{
									currentPlayer = 0;
								}
								else
								{
									currentPlayer++;
								}

							}

						}

					}

				}

			}

		}
							/*
							Console.WriteLine("\nCurrent Player: " + playerList[currentPlayer].GetName());
							Console.WriteLine("\nGame Options" +
									"\n\t1. Check Hand" +
									"\n\t2. Check Discard Pile" +
									"\n\t3. Place Card onto Discard Pile" +
									"\n\t4. End Turn" +
									"\n\t5. Return to Main Menu");
							string playerInput = Read("choice");
							if (playerInput.Equals("1"))
							{
								Console.WriteLine(Hand.GetSequenceDetails(playerList[currentPlayer].GetHand().GetHandList()));
							}
							else if (playerInput.Equals("2"))
							{
								Console.WriteLine(DiscardPile.GetSequenceDetails(1, discard.GetDiscardPileList()));
							}
							else if (playerInput.Equals("3"))
							{
								Hand hand = playerList[currentPlayer].GetHand();
								List<Card> handList = hand.GetHandList();
								List<Card> discardList = discard.GetDiscardPileList();
								while (true)
								{
									int upperBound = hand.GetHandList().Count - 1;
									Console.WriteLine(Hand.GetSequenceDetails(handList));
									int cardInput = int.Parse(Read("choice (index from 0-" + upperBound + ")"));
									if (cardInput < 0 || cardInput > upperBound)
									{
										Console.WriteLine("\nOut of Range (must be between 0 and " + upperBound + " inclusive)");
										continue;
									}
									else if (IsCardSimilar(handList[cardInput], discardList[discardList.Count - 1]))
									{
										discard.ExchangeSequence(discardList, hand.GetCardList(handList[cardInput]));
									}
									else
									{
										Console.WriteLine("\nThis card cannot be placed");
										continue;
									}
								}
								
							}
							else if (playerInput.Equals("4"))
							{
								if (currentPlayer == numOfPlayers-1)
								{
									currentPlayer = 0;
								}
								else
								{
									currentPlayer++;
								}
								
								continue;
							}
							else if (playerInput.Equals("4"))
							{
								break;
							}
							else
							{
								Console.WriteLine("\nInvalid Choice");
								continue;
							}
						}
						
						break;
					}
				}
				else if (menuInput.Equals("2"))
				{
					break;
				}
				else
				{
					Console.WriteLine("\nInvalid Choice");
					continue;
				}
			*/

		public static int ProbeNumOfPlayers()
		{
			while (true)
			{
				int numOfPlayers = int.Parse(Read("number of players"));
				if (numOfPlayers < 2 || numOfPlayers > 4)
				{
					Console.WriteLine("\nUno only supports 2-4 players");
					continue;
				}

				return numOfPlayers;
			}

		}

		public static string Read(string label)
		{
			Console.WriteLine("\nProvide your " + label + ":");
			Console.Write(">");
			string value;
			value = Console.ReadLine();
			return value;

		}

		public static Boolean IsCardSimilar(Card cardInHand, Card cardInDiscard)
		{
			if (cardInHand.GetCardName().Equals(cardInDiscard.GetCardName()) || cardInHand.GetCardType().Equals(cardInDiscard.GetCardType()))
			{
				return true;
			}

			return false;
		}

		public static Boolean IsTurnValid(Card cardInDiscard, List<Card> handList)
		{
			foreach (Card cardInHand in handList)
			{
				if (IsCardSimilar(cardInHand, cardInDiscard))
				{
					return true;
				}

			}

			return false;
		}

	}


	public class WildCard: Card
	{
		/*
		 * WildCard(card_type)
		 * card_type - the name of the card
		 */
		public WildCard(string card_type): base(card_type, null, null)
		{
			//TODO ability
		}

	}

}