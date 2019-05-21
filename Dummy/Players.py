from flask import Flask, request
from flask_cors import CORS
import time
import operator

app = Flask(__name__)
CORS(app)

class Player():

    def __init__(self):
        self.id = None
        self.name = None
        self.team = 0
        self.pointsGame = 0
        self.medalGold = 0
        self.medalSilver = 0
        self.medalBronze = 0
        self.pointsAllGames = 0
    def toJSON(self):
        return {'id': self.id,
                'name': self.name,
                'team': self.team,
                'pointsGame': self.pointsGame,
                'medalGold': self.medalGold,
                'medalSilver': self.medalSilver,
                'medalBronze': self.medalBronze,
                'pointsAllGames': self.pointsAllGames

                }

class Game:
    def __init__(self):
        self.timeCreation = None
        self.players = {}
        self.numberTeams = 0
        self.ipAddress = None
        self.playGame = 'NoGame'
        self.listPlayers = None
        self.shanghaiNumber = 1
        self.playerTurno = 0
        self.playerThrow = 0
        self.currentPlayer = None

games = {}
streaming = {}

def createOrGetGame(ipAddress):
    if ipAddress in games.keys():
        return games[ipAddress]
    else:
        game = Game()
        game.ipAddress = ipAddress
        game.timeCreation =  time.time()
        games[ipAddress] = game

        return game


ip_local = "127.0.0.1"
game = createOrGetGame(ip_local)

for i in range(1,13):
    player1 = Player()
    player1.id = i
    player1.name = "Player N"+str(i)
    game.players[player1.id] = player1
    player1.pointsGame = 0
    player1.team = 0


@app.route('/darts/players', methods=['GET'])
def getPlayers():
    numPlayers = int(request.args.get('numPlayers'))
    numTeams = int(request.args.get('numTeams'))


    game = createOrGetGame(request.environ.get('HTTP_X_REAL_IP', request.remote_addr))
    sortedPlayers = sorted( list(game.players.values()), key=operator.attrgetter('id'))



    asignteam = 1
    playersText = ""
    for i in range(1,numPlayers+1):
        p =game.players[i]
        if(asignteam>numTeams):
            asignteam = 1

        p.team= asignteam
        asignteam+=1

        playersText += str(p.id) +","
        playersText += p.name +","
        playersText += str(p.team) +";"


    return playersText




if __name__ == '__main__':
    app.run(host='localhost', debug=True, port=5000)
