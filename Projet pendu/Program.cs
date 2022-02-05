using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pendu
{
    class Program
    {
        static void Main(string[] args)
        {
            string regles = ChargerRegles("regles.txt");
            List<string> dico = ChargerDico("dicoFR.txt");
            Pendu p = new Pendu(dico, regles);
            p.JouerUnePartie();//c'est la seule fonction qu'on appelle
            Console.ReadLine();
        }
        public static string ChargerRegles(string fichier)
        {
            try
            {
                // Création d'une instance de StreamReader pour permettre la lecture de notre fichier 
                System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("iso-8859-1");
                StreamReader regles = new StreamReader(fichier, encoding);
                string ligne = regles.ReadLine();
                // Lecture de tous les mots du dictionnaire (un par lignes) 
                string r = "";
                while (ligne != null)
                {
                    r += ligne + '\n';
                    ligne = regles.ReadLine();

                }
                // Fermeture du StreamReader 
                regles.Close();
                return r;
            }
            catch (Exception ex)
            {
                // Code exécuté en cas d'exception 
                Console.Write("Une erreur est survenue au cours de la lecture :");
                Console.WriteLine(ex.Message);
                return "";
            }
        }
        public static List<string> ChargerDico(string fichier)
        {
            try
            {
                // Création d'une instance de StreamReader pour permettre la lecture de notre fichier 
                System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("iso-8859-1");
                StreamReader dictionnaire = new StreamReader(fichier, encoding);
                string mot = dictionnaire.ReadLine();
                // Lecture de tous les mots du dictionnaire (un par lignes) 
                List<string> dico = new List<string>();
                while (mot != null)
                {
                    dico.Add(mot);
                    mot = dictionnaire.ReadLine();

                }
                // Fermeture du StreamReader 
                dictionnaire.Close();
                return dico;
            }
            catch (Exception ex)
            {
                // Code exécuté en cas d'exception 
                Console.Write("Une erreur est survenue au cours de la lecture :");
                Console.WriteLine(ex.Message);
                return new List<string>();
            }
        }

        public struct Pendu
        {
            string nomJoueur1, nomJoueur2;
            int mode;//mode de jeu choisi par l'utilisateur
            char[] motCourant;//mot compose des lettres trouvees et de _ designant les lettres manquantes
            string motADeviner;
            List<string> dictionnaire;
            Boolean veutAbandonner;
            Boolean gagne;
            string regles;
            int niveau;
            int essaisRestants;//nombres d'erreurs permises avant d'etre pendu
            string lettreCourante;//lettre proposee par l'utilisateur
            string lettresProposees;//contient les lettres proposees au cours du jeu 
            int nbCoups;// nombre de coups effectues
            int intelligence;//defini le type d'intelligence de la machine
            Random rnd;
            Boolean rejouer;
            int scoreJ1;
            int scoreJ2;
            //string fqltr


            public Pendu(List<string> dico, string r)
            {
                //variables a initialiser dans la fonction jouePartie();

                nomJoueur1 = "";
                nomJoueur2 = "";
                mode = 1;
                motCourant = new char[0];
                motADeviner = "";
                niveau = 1;
                intelligence = 1;

                //-------------autres variables--------------
                dictionnaire = dico;
                veutAbandonner = false;
                gagne = false;
                regles = r;
                lettreCourante = "";
                lettresProposees = "";
                rnd = new Random();
                rejouer = false;
                scoreJ1 = 0;
                scoreJ2 = 0;
                nbCoups = 0;
                essaisRestants = 10;

            }

            public Boolean MotExiste(string mot)
            {//Vérifie que le mot est dans le dictionnaire
                int i = 0;
                while ((i < dictionnaire.Count) && (mot != dictionnaire[i]))
                {
                    i++;
                }
                return (i < dictionnaire.Count);
            }

            public Boolean AppartientMot(string mot, char l)
            {//Vérifie que la lettre est dans le mot en paramètre ou que le mot est égal, si le mot est faux la partie est finie.
                for (int i = 0; i < mot.Length; i++)
                {
                    if (mot[i] == l)
                        return true;
                }
                return false;
            }

            public Boolean DejaProposee(char l)
            { //Renvoie true si la lettre a déjà été proposée, false sinon.
                for (int i = 0; i < lettresProposees.Length; i++)
                {
                    if (lettresProposees[i] == l)
                        return true;
                }
                return false;
            }

            public string MotAleatoire()
            {//Renvoie un mot du dictionnaire.
                int last = dictionnaire.Count;
                int n = rnd.Next(last);
                return dictionnaire[n];
            }

            public char LettreAleatoire()
            {//Renvoie une lettre de l’alphabet aleatoire.
                string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                int alea = rnd.Next(alphabet.Length);
                return alphabet[alea];
            }

            void AjouterLettre()
            {//Lorsqu’une lettre entree par le joueur appartient au mot a deviner, on l’ajoute au mot courant
                for (int i = 0; i < motADeviner.Length; i++)
                {
                    if (lettreCourante[0] == motADeviner[i])
                        motCourant[i] = lettreCourante[0];
                }
            }

            public void Gagne()
            {//la variable gagne devient true si la partie est gagnée et reste false sinon.
                bool g = true;
                int i = 0;
                while ((i < motADeviner.Length) && (g == true))
                {
                    if (motADeviner[i] != motCourant[i])
                        g = false;
                    i++;
                }
                gagne = g;
            }

            void Abandonner()
            {//Change la variable veutAbandonner en true
                veutAbandonner = true;
            }

            public void AfficheTab(char[] tab)
            {// affiche un tableau
                for (int i = 0; i < tab.Length; i++)
                    Console.Write(tab[i] + " ");
            }
            public void Affiche()
            {//Affiche l’avancement de la partie (avec le dessin et les mot à trous)
                Console.WriteLine("Etat du jeu:");
                Console.WriteLine("Lettres proposees: " + lettresProposees);
                Console.Write("Mot a deviner= ");
                AfficheTab(motCourant);
                Console.WriteLine("");
                Console.WriteLine("Potence:");
                Potence();
            }
            public void Potence()
            {//cette fonction dessine la potence
                if (essaisRestants == 9)
                {
                    Console.WriteLine("___");
                }
                else if (essaisRestants == 8)
                {
                    Console.WriteLine("   |       ");
                    Console.WriteLine("   |       ");
                    Console.WriteLine("   |       ");
                    Console.WriteLine(" __|__    ");
                }
                else if (essaisRestants == 7)
                {
                    Console.WriteLine("   _______");
                    Console.WriteLine("   |       ");
                    Console.WriteLine("   |       ");
                    Console.WriteLine("   |       ");
                    Console.WriteLine(" __|__    ");
                }
                else if (essaisRestants == 6)
                {
                    Console.WriteLine("   _______");
                    Console.WriteLine("   |       |");
                    Console.WriteLine("   |       ");
                    Console.WriteLine("   |       ");
                    Console.WriteLine(" __|__    ");
                }
                else if (essaisRestants == 5)
                {
                    Console.WriteLine("   _______");
                    Console.WriteLine("   |       |");
                    Console.WriteLine("   |       o");
                    Console.WriteLine("   |       ");
                    Console.WriteLine(" __|__    ");
                }
                else if (essaisRestants == 4)
                {
                    Console.WriteLine("   _______");
                    Console.WriteLine("   |       |");
                    Console.WriteLine("   |       o");
                    Console.WriteLine("   |       |");
                    Console.WriteLine(" __|__    ");
                }
                else if (essaisRestants == 3)
                {
                    Console.WriteLine("   _______");
                    Console.WriteLine("   |       |");
                    Console.WriteLine("   |       o");
                    Console.WriteLine("   |      -|");
                    Console.WriteLine(" __|__    ");
                }
                else if (essaisRestants == 2)
                {
                    Console.WriteLine("   _______");
                    Console.WriteLine("   |       |");
                    Console.WriteLine("   |       o");
                    Console.WriteLine("   |      -|-");
                    Console.WriteLine(" __|__    ");
                }
                else if (essaisRestants == 1)
                {
                    Console.WriteLine("   _______");
                    Console.WriteLine("   |       |");
                    Console.WriteLine("   |       o");
                    Console.WriteLine("   |      -|-");
                    Console.WriteLine(" __|__    /");
                }
                else if (essaisRestants == 0)
                {
                    Console.WriteLine("   _______");
                    Console.WriteLine("   |       |");
                    Console.WriteLine("   |       o");
                    Console.WriteLine("   |      -|-");
                    Console.WriteLine(" __|__    / \\");
                }

            }

            public Boolean AppartientChar(char c, char[] tab)
            {
                for (int i = 0; i < tab.Length; i++)
                    if (c == tab[i])
                        return true;
                return false;
            }
            int DetermineNiveau(string mot)
            {/*En fonction du mot donné, la fonction renvoie son niveau (longueur du mot, lettres peu communes, rareté du mot, ...) et modifie la variable 
                niveau. Cette fonction est appelée tant que le niveau demandé par le joueur 1 n’est pas respecte*/
                int nv = 0;
                char[] lettres = new char[mot.Length];//tableau comprenant les differentes lettres du mot
                string lettreRare = "QYXJKWZ";
                int nbLettreRare = 0;
                for (int i = 0; i < lettreRare.Length; i++)
                {
                    if (AppartientMot(mot, lettreRare[i]))
                        nbLettreRare++;
                }
                if (mot.Length < 7)
                    nv = 1;
                else if (mot.Length > 6 && mot.Length < 10 && nbLettreRare <= 1)
                    nv = 2;
                else if (mot.Length > 6 && mot.Length < 10 && nbLettreRare > 1)
                    nv = 3;
                else if (mot.Length > 9)
                    nv = 4;
                return nv;

            }

            public void Aide()
            {/*Affiche une aide à l'utilisateur.*/
                if (lettreCourante[0] == '1')
                {//ici l'aide donne une lettre aletoire au joueur
                    int alea = rnd.Next(motADeviner.Length);
                    while (motCourant[alea] != '_')
                        alea = rnd.Next(motADeviner.Length);
                    lettreCourante = "" + motADeviner[alea];
                    AjouterLettre();
                    lettresProposees += (lettreCourante[0]);
                }
                else if (lettreCourante[0] == '2')
                {//cette aide affiche le nombre de lettres, de voyelles et de consonnes du mot
                    int nbLetrresDiff = 0;
                    int nbConsonne = 0;
                    int nbVoyelle = 0;


                    char[] lettres = new char[motADeviner.Length];
                    for (int i = 0; i < motADeviner.Length; i++)
                    {
                        if (!AppartientChar(motADeviner[i], lettres))
                        {
                            lettres[nbLetrresDiff] = motADeviner[i];
                            nbLetrresDiff++;
                        }
                    }
                    for (int i = 0; i < motADeviner.Length; i++)
                    {
                        if (motADeviner[i] == 'A' || motADeviner[i] == 'E' || motADeviner[i] == 'I' || motADeviner[i] == 'O' || motADeviner[i] == 'U' || motADeviner[i] == 'Y')
                            nbVoyelle++;
                        else
                            nbConsonne++;
                    }
                    Console.WriteLine("Le mot que vous essayez de deviner a " + nbLetrresDiff + " lettres differentes dont " + nbVoyelle + " voyelles et " + nbConsonne + " consonnes");

                }
            }

            public void JouerUnTour()
            {//Cette fonction affiche le pendu Demande à l'utilisateur de rentrer une lettre ou un mot, vérifie si cette lettre ou ce mot est valide, en fonction du mode elle peut faire jouer l’ordinateur aussi.
                Affiche();

                if (mode == 2 || mode == 3)// L'utilisateur doit trouver le mot que l'ordinateur a fixé, ou deux joueurs s'affrontent
                {

                    Console.WriteLine(nomJoueur1 + ", veuillez entrer une lettre ou un mot");
                    lettreCourante = Console.ReadLine().ToUpper();//toUpper permet de mettre la chaine en majuscules
                }
                else if (mode == 1 || mode == 4) // L'ordinateur doit trouver le mot que l'utilisateur a proposé, ou deux ordinateurs s'affrontent
                {
                    do
                    {
                        lettreCourante = "" + Intelligence();//l'ordinateur joueura differemment en fonction de son intelligence

                    }
                    while (DejaProposee(lettreCourante[0]));
                    Console.WriteLine("La lettre choisie par " + nomJoueur2 + " est :" + lettreCourante);

                    Console.ReadLine();
                }

                if (lettreCourante[0] == '*')
                    Abandonner();
                else if (lettreCourante[0] == '1' || lettreCourante[0] == '2')
                    Aide();
                else
                {

                    if (lettreCourante.Length > 1)
                    {//ici on gere la cas ou l'utilisateur rentreun mot
                        if (lettreCourante == motADeviner)
                            for (int i = 0; i < motADeviner.Length; i++)
                                motCourant[i] = motADeviner[i];//si le mot est le bon, motCourant est mis a jour avec toutes les bonnes lettres pour que lors de l'appel de la fonction Gagne(), la varable gagne devienne true
                        else
                            essaisRestants = 0;//si le mot n'est pas le bon , la partie est finie
                    }
                    else if (DejaProposee(lettreCourante[0]))
                        essaisRestants--;
                    else if (AppartientMot(motADeviner, lettreCourante[0]))
                    {
                        AjouterLettre();//ajout au motCourant
                        lettresProposees += (lettreCourante[0]);//ajout au lettres deja proposees 
                    }
                    else
                    {//ici la lettre proposee n'est pas dans le mot et n'a pas encore ete proposee
                        essaisRestants--;
                        lettresProposees += lettreCourante[0];
                    }
                }
                Gagne();//on appelle cette fonction pour verifier si la partie est gagnee
            }

            public void InitialiserPartie(/*string nomJ1, string nomJ2, int mode, int niveau, string mot, char t1, char t2*/)
            {/*Initialise la partie, c'est-à- dire les variables correspondant au nom et aux types des joueurs, au mode, au mot a faire deviner, 
                au mot courant au niveau et au nombre d’essais restants. Les variables passées en paramètres permettent d’initialiser toutes les variables.*/
                veutAbandonner = false;
                gagne = false;
                lettreCourante = "";
                lettresProposees = "";
                nbCoups = 0;
                essaisRestants = 10;//Il faut 10 erreurs pour la partie soit perdue
                if (!rejouer)
                {
                    scoreJ1 = 0;
                    scoreJ2 = 0;
                    Console.WriteLine("Saisissez au clavier un mode de jeu:");
                    Console.WriteLine("Mode 1:Vous choisissez un mot et l'ordinateur doit le deviner");
                    Console.WriteLine("Mode 2:Vous essayez de deviner un mot face a l'ordinateur ");
                    Console.WriteLine("Mode 3:Deux joueurs humains s'affrontent, notez que c'est le joueur 2 qui fait deviner un mot a l'autre");
                    Console.WriteLine("Mode 4:Deux ordinateurs s'affrontent");
                    mode = int.Parse(Console.ReadLine());//l'utilisateur choisi un mode de jeu;
                    if (mode == 1 || mode == 2)
                    {
                        Console.WriteLine("Joueur 1: Quel est votre nom?");
                        nomJoueur1 = Console.ReadLine();

                        nomJoueur2 = "Elon Musk";

                    }
                    else if (mode == 3)
                    {
                        Console.WriteLine("Joueur 1: Quel est votre nom?");
                        nomJoueur1 = Console.ReadLine();

                        Console.WriteLine("Joueur 2: Quel est votre nom?");
                        nomJoueur2 = Console.ReadLine();

                    }
                    else if (mode == 4)
                    {
                        nomJoueur1 = "elon Musk";
                        nomJoueur2 = " Jeff Bezos";

                    }


                }

            }
            public void JouerUnePartie()
            {/*Fonction qui gère tout le jeu, demande un mot à l'utilisateur, fonctionnement différent en fonction des modes, 
                (faire plusieurs fonctions jeu en fonction des modes?) qui vérifie si la partie est gagnée ou finie. Il sera possible de rejouer à la fin
                d’une partie.*/
                Console.WriteLine(regles);
                InitialiserPartie();

                if (mode == 1)
                {
                    Console.WriteLine("Votre adversaire est " + nomJoueur2);
                    Console.WriteLine("Choisissez l'intelligence de " + nomJoueur2 + ":");
                    Console.WriteLine("1 - " + nomJoueur2 + " propose des lettres aleatoirement");
                    Console.WriteLine("2 - " + nomJoueur2 + " propose des lettres de la plus utilisee a la moins utilisee en francais");
                    Console.WriteLine("3 - " + nomJoueur2 + " propose des lettres en fonction de leur frequence d'utilisation en francais, " +
                        "plus elles sont utilisees plus elle sont probables d'etre proposees");
                    intelligence = int.Parse(Console.ReadLine());
                    do
                    {//tant que le mot propose n'est pas dans le dictionnaire, on demande au joueur de rentrer un mot
                        Console.WriteLine(nomJoueur1 + ", entrez un mot a faire deviner");
                        motADeviner = Console.ReadLine().ToUpper();

                    } while (!MotExiste(motADeviner));

                }
                else if (mode == 2)
                {
                    Console.WriteLine("Votre adversaire est " + nomJoueur2);
                    Console.WriteLine("Entrez un niveau, entre 1 et 4, le 4 etant le plus dur");
                    niveau = int.Parse(Console.ReadLine());
                    do
                    {//tant que le mot propose par l'ordinateur ne correspond pas au niveau demande par le joueur, l'ordinateur doit proposer un autre mot
                        motADeviner = MotAleatoire();
                    } while (DetermineNiveau(motADeviner) != niveau);

                }
                else if (mode == 3)
                {
                    do
                    {
                        Console.WriteLine(nomJoueur2 + ", entrez un mot a faire deviner");
                        motADeviner = Console.ReadLine().ToUpper();

                    } while (!MotExiste(motADeviner));
                    Console.WriteLine("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n");//On saute des lignes pour que le joueur qui devine ne puisse pas lire le mot entre
                }
                else if (mode == 4)
                {
                    motADeviner = MotAleatoire();
                    Console.WriteLine(nomJoueur1 + " a choisi le mot " + motADeviner);
                    Console.WriteLine("Choisissez l'intelligence de " + nomJoueur2);
                    Console.WriteLine("1 - " + nomJoueur2 + " propose des lettres aléatoirement");
                    Console.WriteLine("2 - " + nomJoueur2 + " propose des lettres de la plus utilisee a la moins utilisee");
                    Console.WriteLine("3 - " + nomJoueur2 + " propose des lettres en fonction de leur frequence d'utilisation en francais");
                    intelligence = int.Parse(Console.ReadLine());
                }
                motCourant = new char[motADeviner.Length];
                for (int i = 0; i < motADeviner.Length; i++)
                    motCourant[i] = '_';//on initialise mot courant avec autant de _ qu'il y a de lettres

                while (essaisRestants > 0 && !gagne && !veutAbandonner)//tant que la partie n'est pas perdue, ni gagnee, ni abandonee, on joue un tour
                {

                    JouerUnTour();
                    nbCoups++;
                }
                Affiche();//a la fin du jeu on afiche l'etat du pendu, puis on affiche qui a gagne
                if (mode == 1 || mode == 4)
                {
                    if (gagne)
                    {
                        Console.WriteLine(nomJoueur2 + " a gagne la manche!!!");
                        scoreJ2++;

                    }
                    else if (essaisRestants == 0)
                    {
                        Console.WriteLine(nomJoueur1 + " a gagne la manche!!!");
                        scoreJ1++;

                    }
                    else if (veutAbandonner)
                        Console.WriteLine("Abandon");
                }
                else if (mode == 2 || mode == 3)
                {
                    if (gagne)
                    {
                        Console.WriteLine(nomJoueur1 + " a gagne!!!");
                        scoreJ1++;

                    }
                    else if (essaisRestants == 0)
                    {
                        Console.WriteLine(nomJoueur2 + " a gagne!!!");
                        Console.WriteLine("Le mot à trouver etait : " + motADeviner);
                        scoreJ2++;
                    }
                    else if (veutAbandonner)
                    {
                        Console.WriteLine("Abandon");
                        Console.WriteLine("Le mot à trouver etait : " + motADeviner);
                    }

                }
                Console.WriteLine(nomJoueur1 + " a " + scoreJ1 + " points et " + nomJoueur2 + " a " + scoreJ2 + " points");
                Console.WriteLine("Voulez vous continuer la partie? repondez par oui ou par non");
                string reponse = Console.ReadLine().ToUpper();
                if (reponse == "OUI")
                {
                    rejouer = true;
                    JouerUnePartie();
                }
                else if (reponse == "NON")
                {
                    Console.WriteLine("Voulez-vous recommencer une partie ?");
                    reponse = Console.ReadLine().ToUpper();
                    if (reponse == "OUI")
                    {
                        rejouer = false;
                        JouerUnePartie();

                    }
                }
            }

            public char Intelligence()
            {//Cette fonction renvoie une lettre en fonction de l'intelligence de la machine
                string chaineOccurence = "EAISNRTOLUDCMPGBVHFQYXJKWZ";//frequence d'apparition des lettres en francais 
                string chaineProba = "EEEEEEEEEEEEEEAAAAAAAIIIIIIISSSSSSSNNNNNNRRRRRRTTTTTTOOOOOLLLLLUUUUDDDDCCCMMMPPGBVHFQYXJKWZ";//sur 100  caracteres on a la frequence de chaque caractere dans la langue francaise
                if (intelligence == 1)//intelligence aleatoire
                {
                    return LettreAleatoire();
                }
                else if (intelligence == 2)//intelligence probabiliste simple 
                {//renvoie les lettres les plus frequentes dans le meme ordre a chaque partie 
                    return chaineOccurence[nbCoups];
                }
                else if (intelligence == 3)// intelligence probabiliste complexe
                {//a plus de chance de renvoyer un lettre frequente dans la langue francaise mais peut aussi renvoyer une lettre peu frequente
                    return chaineProba[rnd.Next(chaineProba.Length)];
                }
                else if (intelligence == 4)
                {

                }
                return LettreAleatoire();
            }

        }

    }
}


