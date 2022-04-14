using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class blackJackController : MonoBehaviour {

    // Contains all information for each card in the deck:
    // One Card struct = one playing card
    public struct Card
    {
        public int Index;
        public int Value;
        public Sprite Sprite;

        public Card(int index, int value, Sprite sprite)
        {
            Index = index;
            Value = value;
            Sprite = sprite;
        }
    }

    //Public variable initialization:
    Text playerScoreDisp;
    Text bankTotalDisp;
    Text instructionDisp;
    Text houseScoreDisp;
    Text resultsDisp;

    Button standButton;
    Button hitButton;
    Button dealButton;

    Image resultsImage;

    public int playerScore;
    public int houseScore;
    public int bankTotal;
    public int playerBet;
    public int currentRound = 0;

    // Create Player and House Decks:
    Stack<Card> fullDeck;
    List<Card> playerDeck = new List<Card>();
    List<Card> houseDeck = new List<Card>();

    // Storing the color of the results text:

    Color red = new Color32(255, 0, 0, 255);
    Color green = new Color32(82, 255, 0, 255);
    Color grey = new Color32(124, 125, 125, 255);

    // Use this for initialization
    void Start()
    {
        // Initially blocks out the stand and hit button
        standButton = GameObject.Find("Canvas/standButton").GetComponent<Button>();
        standButton.interactable = false;

        hitButton = GameObject.Find("Canvas/hitButton").GetComponent<Button>();
        hitButton.interactable = false;

        // Disables the other possible player cards:
        Image playerCard3 = GameObject.Find("Canvas/playerCard3").GetComponent<Image>();
        playerCard3.enabled = false;

        Image playerCard4 = GameObject.Find("Canvas/playerCard4").GetComponent<Image>();
        playerCard4.enabled = false;

        Image playerCard5 = GameObject.Find("Canvas/playerCard5").GetComponent<Image>();
        playerCard5.enabled = false;

        // Disables the other possible house cards:
        Image houseCard3 = GameObject.Find("Canvas/houseCard3").GetComponent<Image>();
        houseCard3.enabled = false;

        Image houseCard4 = GameObject.Find("Canvas/houseCard4").GetComponent<Image>();
        houseCard4.enabled = false;

        Image houseCard5 = GameObject.Find("Canvas/houseCard5").GetComponent<Image>();
        houseCard5.enabled = false;

        Image houseCard6 = GameObject.Find("Canvas/houseCard6").GetComponent<Image>();
        houseCard6.enabled = false;

        Image houseCard7 = GameObject.Find("Canvas/houseCard7").GetComponent<Image>();
        houseCard7.enabled = false;

        // Initialize and display the bank total:
        bankTotal = 100;
        bankTotalDisp = GameObject.Find("Canvas/bankTotalDisp").GetComponent<Text>();
        bankTotalDisp.text = bankTotal.ToString();

        // Initialize house score text:
        houseScoreDisp = GameObject.Find("Canvas/houseScoreDisp").GetComponent<Text>();
        houseScoreDisp.text = "???";
        resultsImage = GameObject.Find("Canvas/resultsImage").GetComponent<Image>();
        resultsImage.enabled = false;

        // Deck Creation using a Stack of Structs
        fullDeck = new Stack<Card>();
        for (int i = 1; i <= 52; i++) {

            Card tempCard = new Card();
            tempCard.Index = i;

            if (i >= 1 && i <= 4) {
                tempCard.Value = 1;
            } else if (i > 4 && i <= 20) {
                tempCard.Value = 10;
            } else if (i > 20 && i <= 24) {
                tempCard.Value = 9;
            } else if (i > 24 && i <= 28) {
                tempCard.Value = 8;
            } else if (i > 28 && i <= 32) {
                tempCard.Value = 7;
            } else if (i > 32 && i <= 36) {
                tempCard.Value = 6;
            } else if (i > 36 && i <= 40) {
                tempCard.Value = 5;
            } else if (i > 40 && i <= 44) {
                tempCard.Value = 4;
            } else if (i > 44 && i <= 48) {
                tempCard.Value = 3;
            } else if (i > 48 && i <= 52) {
                tempCard.Value = 2;
            }

            tempCard.Sprite = Resources.Load<Sprite>(i.ToString());
            fullDeck.Push(tempCard);
        }

        // Shuffle the deck:
        fullDeck = ShuffleDeck(fullDeck);

        // Update Instruction text:
        instructionDisp = GameObject.Find("Canvas/instructionDisp").GetComponent<Text>();
        instructionDisp.text = "Press Deal to start!";
    }
	// Update is called once per frame
	void Update () {
        // Check all the time for if the bank runs out of money:
        if (bankTotal <= 0) {
            // Update results message:
            resultsDisp = GameObject.Find("Canvas/resultsDisp").GetComponent<Text>();
            resultsDisp.text = "You lose!";
            resultsDisp.color = red;
            resultsImage = GameObject.Find("Canvas/resultsImage").GetComponent<Image>();
            resultsImage.enabled = true;

            // Black out the deal button:
            dealButton.interactable = false;

            // Update instruction message:
            instructionDisp = GameObject.Find("Canvas/instructionDisp").GetComponent<Text>();
            instructionDisp.text = "You have run out of money! \n" +
                                    "Press Reset to play again.";
        }

    }

    public void dealButtonClicked()
    {
        // Add one to the current round:
        currentRound++;

        // Deactivate the deal button until the player decides to reset:
        dealButton = GameObject.Find("Canvas/dealButton").GetComponent<Button>();
        dealButton.interactable = false;

        // Reactivate the hit and stand buttons:
        standButton.interactable = true;
        hitButton.interactable = true;

        // If this is not the first round that has been played:
        if (currentRound >=2 ) {
            // Empty player and house decks:
            for(int i = 0; i < playerDeck.Count; i++) {
                fullDeck.Push(playerDeck[i]);
                if (i >= 2) {
                    string nextCardPath = "Canvas/playerCard" + (i + 1);
                    Image nextCardImage = GameObject.Find(nextCardPath).GetComponent<Image>();
                    nextCardImage.enabled = false;
                }
            }
            for (int i = 0; i < houseDeck.Count; i++) {
                fullDeck.Push(houseDeck[i]);
                if (i >= 1) {
                    string nextCardPath = "Canvas/houseCard" + (i + 1);
                    Image nextCardImage = GameObject.Find(nextCardPath).GetComponent<Image>();
                    nextCardImage.enabled = false;
                }
            }
            playerDeck.Clear();
            houseDeck.Clear();

            // Reshuffle the deck:
            fullDeck = ShuffleDeck(fullDeck);

            // Update house and player score:
            updatePlayerScore();
            updateHouseScore();

            // Clear results text:
            resultsDisp = GameObject.Find("Canvas/resultsDisp").GetComponent<Text>();
            resultsDisp.text = " ";
            resultsImage = GameObject.Find("Canvas/resultsImage").GetComponent<Image>();
            resultsImage.enabled = false;

        }

        // Give the player two cards and display them:
        Card newPlayerCard = fullDeck.Pop();
        playerDeck.Add(newPlayerCard);
        Image playerCard1 = GameObject.Find("Canvas/playerCard1").GetComponent<Image>();
        playerCard1.sprite = newPlayerCard.Sprite;

        newPlayerCard = fullDeck.Pop();
        playerDeck.Add(newPlayerCard);
        Image playerCard2 = GameObject.Find("Canvas/playerCard2").GetComponent<Image>();
        playerCard2.sprite = newPlayerCard.Sprite;

        //Give the house two cards and display one of them:
        Card newHouseCard = fullDeck.Pop();
        houseDeck.Add(newHouseCard);
        Image houseCard1 = GameObject.Find("Canvas/houseCard1").GetComponent<Image>();
        houseCard1.sprite = newHouseCard.Sprite;

        newHouseCard = fullDeck.Pop();
        houseDeck.Add(newHouseCard);
        Image houseCard2 = GameObject.Find("Canvas/houseCard2").GetComponent<Image>();
        houseCard2.sprite = Resources.Load<Sprite>("b1fv");
        houseCard2.enabled = true;

        // Calculates the player's current score:
        updatePlayerScore();

        // If they have an Ace but not Blackjack:
        if (checkforAce(playerDeck) != playerScore) {
            playerScoreDisp = GameObject.Find("Canvas/playerScoreDisp").GetComponent<Text>();
            playerScoreDisp.text = playerScore.ToString() + " or " + (checkforAce(playerDeck)).ToString();
        }

        // *** Checks to see if the player has 21 from an ace at the beginning of the game ****
        if (playerScore != checkforAce(playerDeck) && checkforAce(playerDeck) == 21) {   // if they have an Ace and BlackJack:
            if(houseScore != checkforAce(houseDeck) && checkforAce(houseDeck) == 21) {  // If the house also has a natural BlackJack:  
               // Need to reset as if there was a tie:
                dealButton.interactable = true;
                standButton.interactable = false;
                hitButton.interactable = false;

                // Show the second card:
                houseCard2.sprite = houseDeck[1].Sprite;

                // Update results message:
                resultsImage = GameObject.Find("Canvas/resultsImage").GetComponent<Image>();
                resultsImage.enabled = true;
                resultsDisp = GameObject.Find("Canvas/resultsDisp").GetComponent<Text>();
                resultsDisp.text = "It's a tie!\n No one wins :( ";
                resultsDisp.color = grey;

                // Update instruction message:
                instructionDisp = GameObject.Find("Canvas/instructionDisp").GetComponent<Text>();
                instructionDisp.text = "Press Deal to play again \n" +
                                        "or press Reset to clear the bank \n and the entire game over.";

            } else {
                // The player wins - must deal again or restart:
                dealButton.interactable = true;
                standButton.interactable = false;
                hitButton.interactable = false;

                // Get the user bet:
                InputField inputFieldBet = GameObject.Find("Canvas/playerBetInputField").GetComponent<InputField>();
                if (string.IsNullOrEmpty(inputFieldBet.text)) {
                    playerBet = 10;
                } else {
                    playerBet = System.Convert.ToInt32(inputFieldBet.text);
                }

                // Update the bank amount:
                bankTotal = bankTotal + playerBet;
                bankTotalDisp = GameObject.Find("Canvas/bankTotalDisp").GetComponent<Text>();
                bankTotalDisp.text = bankTotal.ToString();

                // Show the second card:
                houseCard2.sprite = houseDeck[1].Sprite;
                updateHouseScore();

                // Update results message:
                resultsDisp = GameObject.Find("Canvas/resultsDisp").GetComponent<Text>();
                resultsDisp.text = "BLACKJACK!!!\n You win!";
                resultsDisp.color = green;
                resultsImage = GameObject.Find("Canvas/resultsImage").GetComponent<Image>();
                resultsImage.enabled = true;

                // Update instruction message:
                instructionDisp = GameObject.Find("Canvas/instructionDisp").GetComponent<Text>();
                instructionDisp.text = "Press Deal to play again \n" +
                                        "or press Reset to clear the bank \n and the entire game over.";
            }
        }
       
        // If they do not have an Ace, update instruction text:
        instructionDisp = GameObject.Find("Canvas/instructionDisp").GetComponent<Text>();
        instructionDisp.text = "Press Hit to get another card\n" +
                               "Press Stand to finish the round\n" +
                               "Press Reset to clear the board \n" +
                               "and start a new game.";

    }

    public void standButtonClicked()
    {
        // Deactivate stand and hit buttons and activate deal button:
        dealButton.interactable = true;
        standButton.interactable = false;
        hitButton.interactable = false;

        // Dealer's next card is flipped face up:
        int nextCardNum = houseDeck.Count;
        Card nextCard = houseDeck[(nextCardNum - 1)];
        Image nextCardImage = GameObject.Find("Canvas/houseCard" + nextCardNum).GetComponent<Image>();
        nextCardImage.sprite = nextCard.Sprite;

        // Calculate dealer's score:
        updateHouseScore();

        // Until the the dealer's score equals 17 or more:
        while (houseScore < 17) {

            // Check to see if the house has an ace and break if 17 or more
            if (houseScore != checkforAce(houseDeck)) {
                if (checkforAce(houseDeck) >= 17 && checkforAce(houseDeck) <= 21) {
                    break;
                }
            }

            // Add another card to the house's hand:
            Card newNextCard = fullDeck.Pop();
            houseDeck.Add(newNextCard);
            nextCardNum = houseDeck.Count;

            // Give the sprite to the right game image
            Image newNextImage = GameObject.Find("Canvas/houseCard" + nextCardNum).GetComponent<Image>();
            newNextImage.sprite = newNextCard.Sprite;
            newNextImage.enabled = true;

            // Update house's total:
            updateHouseScore();
        }

        // Get the user bet:
        InputField inputFieldBet = GameObject.Find("Canvas/playerBetInputField").GetComponent<InputField>();
        if (string.IsNullOrEmpty(inputFieldBet.text)) {
            playerBet = 10;
        } else {
            playerBet = System.Convert.ToInt32(inputFieldBet.text);
        }

        // Update house's score for an ace:
        if (checkforAce(houseDeck) != houseScore) {
            // If it would not cause the player to bust, then update score for the ace:
            if (checkforAce(houseDeck) <= 21) {
                houseScore = checkforAce(houseDeck);
                houseScoreDisp = GameObject.Find("Canvas/houseScoreDisp").GetComponent<Text>();
                houseScoreDisp.text = houseScore.ToString();
            }
        }

        // Check to see if the player has an ace:
        if (checkforAce(playerDeck) != playerScore) {
            // If it would not cause the player to bust, then update score for the ace:
            if (checkforAce(playerDeck) <= 21) {
                playerScore = checkforAce(playerDeck);
                playerScoreDisp = GameObject.Find("Canvas/playerScoreDisp").GetComponent<Text>();
                playerScoreDisp.text = playerScore.ToString();
            }
        }
            
        // First check to see if the house busts and the player didn't:
        if (houseScore > 21 && playerScore <= 21) {
            // Update the bank amount:
            bankTotal = bankTotal + playerBet;
            bankTotalDisp = GameObject.Find("Canvas/bankTotalDisp").GetComponent<Text>();
            bankTotalDisp.text = bankTotal.ToString();

            // Update results message:
            resultsDisp = GameObject.Find("Canvas/resultsDisp").GetComponent<Text>();
            resultsDisp.text = "The house busted.\n You win!";
            resultsDisp.color = green;
            resultsImage = GameObject.Find("Canvas/resultsImage").GetComponent<Image>();
            resultsImage.enabled = true;

            // Update instruction message:
            instructionDisp = GameObject.Find("Canvas/instructionDisp").GetComponent<Text>();
            instructionDisp.text = "Press Deal to play again \n" +
                                    "or press Reset to clear the bank\n and start the entire game over.";

            // If the player busts and the house didn't:
        } else if (houseScore <= 21 && playerScore > 21) {
            // Update the bank amount:
            bankTotal = bankTotal - playerBet;
            bankTotalDisp = GameObject.Find("Canvas/bankTotalDisp").GetComponent<Text>();
            bankTotalDisp.text = bankTotal.ToString();

            // Update results message:
            resultsDisp = GameObject.Find("Canvas/resultsDisp").GetComponent<Text>();
            resultsDisp.text = "You busted.\n You lose!";
            resultsDisp.color = red;
            resultsImage = GameObject.Find("Canvas/resultsImage").GetComponent<Image>();
            resultsImage.enabled = true;

            // Update instruction message:
            instructionDisp = GameObject.Find("Canvas/instructionDisp").GetComponent<Text>();
            instructionDisp.text = "Press Deal to play again \n" +
                                    "or press Reset to clear the bank \n and start the entire game over.";

            // If they both bust:
        } else if (houseScore > 21 && playerScore > 21) {
            // Update results message:
            resultsDisp = GameObject.Find("Canvas/resultsDisp").GetComponent<Text>();
            resultsDisp.text = "You both busted!\n No one wins :( ";
            resultsDisp.color = grey;
            resultsImage = GameObject.Find("Canvas/resultsImage").GetComponent<Image>();
            resultsImage.enabled = true;

            // Update instruction message:
            instructionDisp = GameObject.Find("Canvas/instructionDisp").GetComponent<Text>();
            instructionDisp.text = "Press Deal to play again \n" +
                                    "or press Reset to clear the bank \n and the entire game over.";

            // If they both did not bust:
        } else if (houseScore <= 21 && playerScore <= 21) { 

            // If the user wins:
            if (playerScore > houseScore) {
                // Update the bank amount:
                bankTotal = bankTotal + playerBet;
                bankTotalDisp = GameObject.Find("Canvas/bankTotalDisp").GetComponent<Text>();
                bankTotalDisp.text = bankTotal.ToString();

                // Update results message:
                resultsDisp = GameObject.Find("Canvas/resultsDisp").GetComponent<Text>();
                resultsDisp.text = "You win!";
                resultsDisp.color = green;
                resultsImage = GameObject.Find("Canvas/resultsImage").GetComponent<Image>();
                resultsImage.enabled = true;

                // Update instruction message:
                instructionDisp = GameObject.Find("Canvas/instructionDisp").GetComponent<Text>();
                instructionDisp.text = "Press Deal to play again \n" +
                                        "or press Reset to clear the bank \n and the entire game over.";

                // If the house wins:
            } else if (playerScore < houseScore) {
                // Update the bank amount:
                bankTotal = bankTotal - playerBet;
                bankTotalDisp = GameObject.Find("Canvas/bankTotalDisp").GetComponent<Text>();
                bankTotalDisp.text = bankTotal.ToString();

                if (bankTotal <= 0) {
                    // Update results message:
                    resultsDisp = GameObject.Find("Canvas/resultsDisp").GetComponent<Text>();
                    resultsDisp.text = "You lose!";
                    resultsDisp.color = red;
                    resultsImage = GameObject.Find("Canvas/resultsImage").GetComponent<Image>();
                    resultsImage.enabled = true;

                    // Black out the deal button:
                    dealButton.interactable = false;

                    // Update instruction message:
                    instructionDisp = GameObject.Find("Canvas/instructionDisp").GetComponent<Text>();
                    instructionDisp.text = "You have run out of money! \n" +
                                            "Press Reset to play again.";
                } else {
                    // Update results message:
                    resultsDisp = GameObject.Find("Canvas/resultsDisp").GetComponent<Text>();
                    resultsDisp.text = "You lose!";
                    resultsDisp.color = red;
                    resultsImage = GameObject.Find("Canvas/resultsImage").GetComponent<Image>();
                    resultsImage.enabled = true;

                    // Update instruction message:
                    instructionDisp = GameObject.Find("Canvas/instructionDisp").GetComponent<Text>();
                    instructionDisp.text = "Press Deal to play again \n" +
                                            "or press Reset to clear the bank\n and start the entire game over.";
                }

                // If there is a tie:
            } else if (playerScore == houseScore) {
                // Update results message:
                resultsDisp = GameObject.Find("Canvas/resultsDisp").GetComponent<Text>();
                resultsDisp.text = "It's a tie!\n No one wins :( ";
                resultsDisp.color = grey;
                resultsImage = GameObject.Find("Canvas/resultsImage").GetComponent<Image>();
                resultsImage.enabled = true;

                // Update instruction message:
                instructionDisp = GameObject.Find("Canvas/instructionDisp").GetComponent<Text>();
                instructionDisp.text = "Press Deal to play again \n" +
                                        "or press Reset to clear the bank \n and the entire game over.";

            }
        }

    }

    public void hitButtonClicked()
    {
        // Activate and deactivate the right buttons:
        dealButton.interactable = false;
        standButton.interactable = true;
        hitButton.interactable = true;

        // Deal the player another card:
        Card nextPlayerCard = fullDeck.Pop();
        playerDeck.Add(nextPlayerCard);
        int playerDeckSize = playerDeck.Count;

        // Give the sprite to the right game image
        Image newNextImage = GameObject.Find("Canvas/playerCard" + playerDeckSize).GetComponent<Image>();
        newNextImage.sprite = nextPlayerCard.Sprite;
        newNextImage.enabled = true;

        // Calculate the player's new score:
        updatePlayerScore();

        // If they have an Ace, change score text:
        if (checkforAce(playerDeck) != playerScore) {
            playerScoreDisp = GameObject.Find("Canvas/playerScoreDisp").GetComponent<Text>();
            playerScoreDisp.text = playerScore.ToString() + " or " + (checkforAce(playerDeck)).ToString();
        }

        // Determine whether the player busted or not:
        if (playerScore > 21) {  
            // Get the user bet:
            InputField inputFieldBet = GameObject.Find("Canvas/playerBetInputField").GetComponent<InputField>();
            if (string.IsNullOrEmpty(inputFieldBet.text)) {
                playerBet = 10;
            } else {
                playerBet = System.Convert.ToInt32(inputFieldBet.text);
            }

            // Substract bet from the bank:
            bankTotal = bankTotal - playerBet;
            bankTotalDisp = GameObject.Find("Canvas/bankTotalDisp").GetComponent<Text>();
            bankTotalDisp.text = bankTotal.ToString();

            // Force the player to only deal or reset:
            dealButton.interactable = true;
            standButton.interactable = false;
            hitButton.interactable = false;

            // Update results text:
            resultsDisp = GameObject.Find("Canvas/resultsDisp").GetComponent<Text>();
            resultsDisp.text = "You've busted!";
            resultsDisp.color = red;
            resultsImage = GameObject.Find("Canvas/resultsImage").GetComponent<Image>();
            resultsImage.enabled = true;

            // Update instruction text:
            instructionDisp = GameObject.Find("Canvas/instructionDisp").GetComponent<Text>();
            instructionDisp.text = "Press Deal to play again \n" +
                                    "or press Reset to clear the bank \n" +
                                    "and the entire game over.";
        } else {
            // Update instruction text:
            instructionDisp = GameObject.Find("Canvas/instructionDisp").GetComponent<Text>();
            instructionDisp.text = "Press Hit to get another card \n" +
                                    "Press Stand to end the round \n" +
                                    "or Press Reset to restart the entire game.\n";
        }
       
    }
    
    public void resetButtonClicked()
    {
        // Reset the current round back to 0:
        currentRound = 0;

        // Return player and house hands to the deck and disable the extra cards:
        for (int i = 0; i < playerDeck.Count; i++) {
            fullDeck.Push(playerDeck[i]);
            if (i >= 2) {
                string nextCardPath = "Canvas/playerCard" + (i + 1);
                Image nextCardImage = GameObject.Find(nextCardPath).GetComponent<Image>();
                nextCardImage.enabled = false;
            }
        }
        for (int i = 0; i < houseDeck.Count; i++) {
            fullDeck.Push(houseDeck[i]);
            if (i >= 1) {
                string nextCardPath = "Canvas/houseCard" + (i + 1);
                Image nextCardImage = GameObject.Find(nextCardPath).GetComponent<Image>();
                nextCardImage.enabled = false;
            }
        }

        // Empty the player and house deck:
        playerDeck.Clear();
        houseDeck.Clear();

        // Reshuffle the deck:
        fullDeck = ShuffleDeck(fullDeck);

        // Update house and player score:
        updatePlayerScore();
        updateHouseScore();

        // If they have an Ace but not Blackjack:
        if (checkforAce(playerDeck) != playerScore) {
            playerScoreDisp = GameObject.Find("Canvas/playerScoreDisp").GetComponent<Text>();
            playerScoreDisp.text = playerScore.ToString() + " or " + (checkforAce(playerDeck)).ToString();
        }

        // Change the first four cards the back of card image:
        Image houseCard1 = GameObject.Find("Canvas/houseCard1").GetComponent<Image>();
        houseCard1.sprite = Resources.Load<Sprite>("b1fv");
        houseCard1.enabled = true;

        Image houseCard2 = GameObject.Find("Canvas/houseCard2").GetComponent<Image>();
        houseCard2.sprite = Resources.Load<Sprite>("b1fv");
        houseCard2.enabled = true;

        Image playerCard1 = GameObject.Find("Canvas/playerCard1").GetComponent<Image>();
        playerCard1.sprite = Resources.Load<Sprite>("b1fv");
        playerCard1.enabled = true;

        Image playerCard2 = GameObject.Find("Canvas/playerCard2").GetComponent<Image>();
        playerCard2.sprite = Resources.Load<Sprite>("b1fv");
        playerCard2.enabled = true;

        // Reset the bank to the default value:
        bankTotal = 100;
        bankTotalDisp = GameObject.Find("Canvas/bankTotalDisp").GetComponent<Text>();
        bankTotalDisp.text = bankTotal.ToString();

        // Reset the bet to the default value:
        InputField inputFieldBet = GameObject.Find("Canvas/playerBetInputField").GetComponent<InputField>();
        inputFieldBet.text = string.Empty;
        playerBet = 10;

        // Grey out all buttons but deal:
        dealButton.interactable = true;
        standButton.interactable = false;
        hitButton.interactable = false;

        // Reset Results text:
        resultsDisp = GameObject.Find("Canvas/resultsDisp").GetComponent<Text>();
        resultsDisp.text = " ";
        resultsImage = GameObject.Find("Canvas/resultsImage").GetComponent<Image>();
        resultsImage.enabled = false;

        // Reset player and house score text:
        houseScoreDisp = GameObject.Find("Canvas/houseScoreDisp").GetComponent<Text>();
        houseScoreDisp.text = "???";

        playerScoreDisp = GameObject.Find("Canvas/playerScoreDisp").GetComponent<Text>();
        playerScoreDisp.text = "???";

        // Reset Instruction text:
        instructionDisp = GameObject.Find("Canvas/instructionDisp").GetComponent<Text>();
        instructionDisp.text = "Press Deal to start!";
    }

    // Shuffles the deck at the beginning of each game:
    public static Stack<Card> ShuffleDeck(Stack<Card> notShuffled)
    {
        Stack<Card> Shuffled = new Stack<Card>();

        Card[] arr = notShuffled.ToArray();
        for (int i = arr.Length; i > 1; i--) {
            // Pick random element to swap.
            int j = Random.Range(0, i); // 0 <= j <= i-1
                                        // Swap.
            Card tmp = arr[j];
            arr[j] = arr[i - 1];
            arr[i - 1] = tmp;
            Shuffled.Push(arr[i - 1]);
        }
        return Shuffled;
    }

    // Calculates and displays the player's current score:
    public void updatePlayerScore()
    {
        playerScoreDisp = GameObject.Find("Canvas/playerScoreDisp").GetComponent<Text>();

        if (playerDeck.Count == 0) {
            playerScore = 0;
            playerScoreDisp.text = "???";
        } else {
            playerScore = 0;
            foreach (Card pc in playerDeck) {
                playerScore = pc.Value + playerScore;
            }
            playerScoreDisp.text = playerScore.ToString();
        }
    }
    
    // Calculates and displays the house's current score:
    public void updateHouseScore()
    {
        houseScoreDisp = GameObject.Find("Canvas/houseScoreDisp").GetComponent<Text>();
        if(houseDeck.Count == 0) {
            houseScore = 0;
            houseScoreDisp.text = "???";
        } else {
            houseScore = 0;
            foreach (Card pc in houseDeck) {
                houseScore = pc.Value + houseScore;
            }

            houseScoreDisp.text = houseScore.ToString();
        }
       
    }

    //Checks to see if the given hand contains an ace and makes the proper update:
    public static int checkforAce(List<Card> hand)
    {
        int updateHandValue = 0;
        for (int i = 0; i < hand.Count; i++) {
            if(hand[i].Value == 1) {
                updateHandValue = 11 + updateHandValue;

            } else {
                updateHandValue = hand[i].Value + updateHandValue;
            }
        }
        return updateHandValue;
    }

}
