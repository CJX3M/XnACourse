using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XnaCards;

namespace ProgrammingAssignment6
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        const int WindowWidth = 800;
        const int WindowHeight = 600;

        // max valid blockjuck score for a hand
        const int MaxHandValue = 21;

        // deck and hands
        Deck deck;
        List<Card> dealerHand = new List<Card>();
        List<Card> playerHand = new List<Card>();

        // hand placement
        const int TopCardOffset = 100;
        const int HorizontalCardOffset = 150;
        const int VerticalCardSpacing = 125;

        // messages
        SpriteFont messageFont;
        const string ScoreMessagePrefix = "Score: ";
        Message playerScoreMessage;
        Message dealerScoreMessage;
        Message winnerMessage;
		List<Message> messages = new List<Message>();

        // message placement
        const int ScoreMessageTopOffset = 25;
        const int HorizontalMessageOffset = HorizontalCardOffset;
        Vector2 winnerMessageLocation = new Vector2(WindowWidth / 2,
            WindowHeight / 2);

        // menu buttons
        Texture2D quitButtonSprite;
        List<MenuButton> menuButtons = new List<MenuButton>();

        // menu button placement
        const int TopMenuButtonOffset = TopCardOffset;
        const int QuitMenuButtonOffset = WindowHeight - TopCardOffset;
        const int HorizontalMenuButtonOffset = WindowWidth / 2;
        const int VeryicalMenuButtonSpacing = 125;

        // use to detect hand over when player and dealer didn't hit
        bool playerHit = false;
        bool dealerHit = false;

        // game state tracking
        static GameState currentState = GameState.WaitingForPlayer;

        // final message to display
        private string finalMessage;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // set resolution and show mouse
            graphics.PreferredBackBufferHeight = WindowHeight;
            graphics.PreferredBackBufferWidth = WindowWidth;
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // create and shuffle deck
            deck = new Deck(Content, HorizontalCardOffset, TopCardOffset);
            deck.Shuffle();

            // first player card
            playerHand.Add(deck.TakeTopCard());
            playerHand[0].FlipOver();
            // first dealer card
            dealerHand.Add(deck.TakeTopCard());

            // second player card
            playerHand.Add(deck.TakeTopCard());
            playerHand[1].FlipOver();
            // second dealer card
            dealerHand.Add(deck.TakeTopCard());
            dealerHand[1].FlipOver();

            // load sprite font, create message for player score and add to list
            messageFont = Content.Load<SpriteFont>(@"fonts\Arial24");
            playerScoreMessage = new Message(ScoreMessagePrefix + GetBlockjuckScore(playerHand).ToString(),
                messageFont,
                new Vector2(HorizontalMessageOffset, ScoreMessageTopOffset));
            messages.Add(playerScoreMessage);

            // load quit button sprite for later use
			quitButtonSprite = Content.Load<Texture2D>(@"graphics\quitbutton");

            // create hit button and add to list
            menuButtons.Add(new MenuButton(Content.Load<Texture2D>(@"graphics\hitbutton"), new Vector2(HorizontalMenuButtonOffset, TopCardOffset), GameState.PlayerHitting));

            // create stand button and add to list
            menuButtons.Add(new MenuButton(Content.Load<Texture2D>(@"graphics\standbutton"), new Vector2(HorizontalMenuButtonOffset, TopCardOffset + VerticalCardSpacing), GameState.WaitingForDealer));

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            MouseState mouseState = Mouse.GetState();
            // update menu buttons as appropriate
            if (currentState == GameState.WaitingForPlayer || currentState == GameState.DisplayingHandResults)
            {
                foreach (MenuButton btn in menuButtons)
                {
                    btn.Update(mouseState);
                }
            }

            // game state-specific processing
            switch(currentState)                
            {
                case GameState.PlayerHitting:
                    Card newCardPlayer = deck.TakeTopCard();
                    playerHand.Add(newCardPlayer);
                    newCardPlayer.FlipOver();
                    messages[0].Text = GetBlockjuckScore(playerHand).ToString();
                    currentState = GameState.WaitingForDealer;
                    playerHit = true;
                    break;
                case GameState.DealerHitting:
                    Card newCardDealer = deck.TakeTopCard();
                    dealerHand.Add(newCardDealer);
                    newCardDealer.FlipOver();
                    dealerHit = true;
                    currentState = GameState.CheckingHandOver;
                    break;
                case GameState.WaitingForDealer:
                    if (GetBlockjuckScore(dealerHand) < 17)
                        currentState = GameState.DealerHitting;
                    else
                    {
                        dealerHit = false;
                        currentState = GameState.CheckingHandOver;
                    }
                    break;
                case GameState.CheckingHandOver:
                    int playerScore = GetBlockjuckScore(playerHand);
                    int dealerScore = GetBlockjuckScore(dealerHand);
                    #region Player or dealer Busted
                    // player loses
                    if (playerScore > MaxHandValue && dealerScore < MaxHandValue) {
                        finalMessage = "Dealer WINS";
                        currentState = GameState.DisplayingHandResults;
                    }
                    // dealer loses
                    else if (playerScore < MaxHandValue && dealerScore > MaxHandValue) {
                        finalMessage = "Player WINS";
                        currentState = GameState.DisplayingHandResults;
                    }
                    else if (playerScore > MaxHandValue && dealerScore > MaxHandValue)
                    {
                        finalMessage = "Both LOSE";
                        currentState = GameState.DisplayingHandResults;
                    }
                    #endregion
                    // player wins
                    //else if (playerScore == 21) { }
                    // dealer wins
                    //else if (dealerScore == 21) { }
                    #region Both player and dealer stand
                    if (currentState != GameState.DisplayingHandResults)
                    {
                        if (!dealerHit && !playerHit)
                        {
                            // player wins
                            if (playerScore > dealerScore)
                            {
                                finalMessage = "Player WINS";
                                currentState = GameState.DisplayingHandResults;
                            }
                            // dealer wins
                            else if (playerScore < dealerScore)
                            {
                                finalMessage = "Dealer WINS";
                                currentState = GameState.DisplayingHandResults;
                            }
                            // tie
                            else if (playerScore == dealerScore)
                            {
                                finalMessage = "TIE";
                                currentState = GameState.DisplayingHandResults;
                            }
                        }
                        #endregion
                        #region Game carries on
                        else
                        {
                            dealerHit = false;
                            playerHit = false;
                            currentState = GameState.WaitingForPlayer;
                        }
                        #endregion
                    }
                    break;
                case GameState.DisplayingHandResults:
                    if(!dealerHand[0].FaceUp)
                        dealerHand[0].FlipOver();
                    dealerScoreMessage = new Message(string.Format("{0} {1}",
                        ScoreMessagePrefix, GetBlockjuckScore(dealerHand)), messageFont, new Vector2(WindowWidth - HorizontalMessageOffset, ScoreMessageTopOffset));
                    messages.Add(dealerScoreMessage);
                    winnerMessage = new Message(finalMessage, messageFont, winnerMessageLocation);
                    messages.Add(winnerMessage);
                    menuButtons.Clear();
                    menuButtons.Add(new MenuButton(quitButtonSprite, new Vector2(WindowWidth / 2, QuitMenuButtonOffset), GameState.Exiting));
                    currentState = GameState.WaitingForPlayer;
                    break;
                case GameState.Exiting:
                    Exit();
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Goldenrod);
						
            spriteBatch.Begin();

            // draw hands
            for(int i = 0; i < playerHand.Count; i++)
            {
                Card card = playerHand[i];
                card.X = HorizontalCardOffset;
                card.Y = TopCardOffset + (VerticalCardSpacing * i);
                card.Draw(spriteBatch);
            }
            for (int i = 0; i < dealerHand.Count; i++)
            {
                Card card = dealerHand[i];
                card.X = WindowWidth - HorizontalCardOffset;
                card.Y = TopCardOffset + (VerticalCardSpacing * i);
                card.Draw(spriteBatch);
            }
            // draw messages
            foreach (Message msg in messages)
            {
                msg.Draw(spriteBatch);
            }

            // draw menu buttons
            foreach(MenuButton btn in menuButtons)
            {
                btn.Draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Calculates the Blockjuck score for the given hand
        /// </summary>
        /// <param name="hand">the hand</param>
        /// <returns>the Blockjuck score for the hand</returns>
        private int GetBlockjuckScore(List<Card> hand)
        {
            // add up score excluding Aces
            int numAces = 0;
            int score = 0;
            foreach (Card card in hand)
            {
                if (card.Rank != Rank.Ace)
                {
                    score += GetBlockjuckCardValue(card);
                }
                else
                {
                    numAces++;
                }
            }

            // if more than one ace, only one should ever be counted as 11
            if (numAces > 1)
            {
                // make all but the first ace count as 1
                score += numAces - 1;
                numAces = 1;
            }

            // if there's an Ace, score it the best way possible
            if (numAces > 0)
            {
                if (score + 11 <= MaxHandValue)
                {
                    // counting Ace as 11 doesn't bust
                    score += 11;
                }
                else
                {
                    // count Ace as 1
                    score++;
                }
            }

            return score;
        }

        /// <summary>
        /// Gets the Blockjuck value for the given card
        /// </summary>
        /// <param name="card">the card</param>
        /// <returns>the Blockjuck value for the card</returns>
        private int GetBlockjuckCardValue(Card card)
        {
            switch (card.Rank)
            {
                case Rank.Ace:
                    return 11;
                case Rank.King:
                case Rank.Queen:
                case Rank.Jack:
                case Rank.Ten:
                    return 10;
                case Rank.Nine:
                    return 9;
                case Rank.Eight:
                    return 8;
                case Rank.Seven:
                    return 7;
                case Rank.Six:
                    return 6;
                case Rank.Five:
                    return 5;
                case Rank.Four:
                    return 4;
                case Rank.Three:
                    return 3;
                case Rank.Two:
                    return 2;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Changes the state of the game
        /// </summary>
        /// <param name="newState">the new game state</param>
        public static void ChangeState(GameState newState)
        {
            currentState = newState;
        }
    }
}
