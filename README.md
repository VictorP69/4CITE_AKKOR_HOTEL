# Installation du projet 

A la racine du projet : ```docker compose watch```

Il faudra par la suite stopper les conteneurs (```CTRL + C```) et relancer ```docker compose watch``` (l'api crash a la première installation car la db met du temps à s'initilisater et il ne l'a trouve pas)

Frontend: http://localhost:8000/
Api (swagger): http://localhost8081/swagger/index.html


# Execution des tests

Vous pouvez tentez d'ouvrir une PR (en partant de main)/ faire un commit sur le github sur la branche main : https://github.com/VictorP69/4CITE_AKKOR_HOTEL

Si vous souhaitez lancer les tests localement (déconseillé) :
- Aller dans frontend : ```npm i```

- Aller dans API/Tests : ```dotnet test```
Après chaque test local dotnet il faudra lancer (à cause du fichier assembly dupliqué) : ```dotnet clean```

Les tests e2e ne fonctionnent pas sur la CI car le frontend n'a pas accès a l'API sur la machine distante
Pour cela vous pouvez lancer les conteneurs avec docker compose watch à la racine du projet et aller dans /frontend/ et lancer ```npx cypress run``` (si vous avez cypress en local)

# CI

La CI éxécute les jobs run_backend_tests et run_frontend_tests qui sont les tests unitaires/fonctionnels
Par la suite run_cypress_tests est lancé -> Cela va lancer le front sur la machine ubuntu et lancer ```npx cypress run``` mais cela ne fonctionne pas car le front n'arrive pas à accéder à l'API