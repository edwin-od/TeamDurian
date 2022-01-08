Ou modifier quand il y a un nouveau EnemyPattern ajouté?

1. Class Level.cs ->
public enum Pattern : Ajouter enum avec nom du nouveau pattern

2. Class Level.cs ->
Ajouter static Color pour l'editeur, et dans la methode de retour de la couleur et du string (juste en dessous)

3. Class Level.cs ->
L'ajouter aussi dans le dropdown

4. Class Level.cs ->
L'ajouter aussi dans la legende

5. ScriptableObject Levels ->
L'ajouter aussi dans la liste des patterns