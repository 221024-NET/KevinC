using System;
using PokemonApp;

namespace Program{

    class Program{

        static Pokemon [] pokeList = new Pokemon [5];

        static void Main(string[] args)
        {
            //Initializing an object
            //We call the constructor, and pass it the desired values for this object
            //                             Name , Dex# , Type, Health, Damage, Ability
            Pokemon pikachu = new Pokemon("Pikachu", 1, "Electric", 12, 2, "Static");
            Pokemon charizard = new Pokemon("Charizard", 2, "Fire", 20, 3, "Blaze");
            Pokemon pidgeot = new Pokemon("Pidgeot", 3, "Normal", 15, 4, "Hurricane");
            Pokemon squirtle = new Pokemon("Squirtle", 4, "Water", 14, 5, "Water Squirt");
            Pokemon rattata = new Pokemon("Rattata", 5, "Normal", 10, 3, "Bite");

            pokeList[0] = pikachu;
            pokeList[1] = charizard; 
            pokeList[2] = pidgeot;
            pokeList[3] = squirtle;
            pokeList[4] = rattata;
            //pokeList[5] = pidgeot;

            Pokemon user = promptUser();
            Console.WriteLine("User chose " + user.name);

            Pokemon opp = getOpponentPokemon(user);
            Console.WriteLine("Opponent chose " + opp.name);

            Thread.Sleep(2000); // = 2 seconds

            Console.WriteLine("Winner: " + fightPokemon(user, opp).name);
        }

        static Pokemon promptUser() {
            for (int i = 0; i < pokeList.Length; i++) {
                Console.WriteLine("[" + pokeList[i].DexNumber + "] " + pokeList[i].name);
            }

            int num = 0;
            do {
                Console.WriteLine("Please Choose a Pokemon (Type a number between 1 and 5)");
                try {
                    string input = Console.ReadLine();
                    num = Int32.Parse(input);
                } catch (Exception e) {
                    Console.WriteLine("PLEASE Choose a Pokemon (Type a number between 1 and 5)");
                }
            } while(num < 1 || num > 5);

            return pokeList[num-1];
        }

        static Pokemon getOpponentPokemon(Pokemon humanPlayer) {
            var rand = new Random();
            Pokemon chosen;
            do {
                int i = rand.Next(pokeList.Length);
                chosen = pokeList[i];
            } while (chosen == humanPlayer);
            return chosen;
        }

        static Pokemon fightPokemon(Pokemon player1, Pokemon player2) {
            do {
                player2.health = player2.health - player1.damage;
                Console.WriteLine(player1.name + " attacks with " + player1.ability + " and " + player2.name + " is dealt " + player1.damage + " damage.");
                Thread.Sleep(2000);
                Console.WriteLine(" " + player2.name + " has " + player2.health + " health remaining.");
                Thread.Sleep(2000);
                if (player2.health > 0) {
                    player1.health = player1.health - player2.damage;
                    Console.WriteLine(player2.name + " attacks with " + player2.ability + " and " + player1.name + " is dealt " + player2.damage + " damage.");
                    Thread.Sleep(2000);
                    Console.WriteLine(" " + player1.name + " has " + player1.health + " health remaining.");
                    Thread.Sleep(2000);
                }
            } while (player1.health > 0 && player2.health > 0);

            if (player1.health <= 0) {
                return player2;
            } else {
                return player1;
            }
        }
    }

}