import websockets
import asyncio
from random import randint

TIME_TO_WAIT_HITS = 2
TIME_TO_WAIT_PLAYERS = 2.5
RATIO_MISS = 6
RATIO_REMOVE_DARTS = 3

sectionsColors = [
"19-W-1",
"16-W-1",
"17-W-1",
"11-W-1",
"19-W-2",
"16-W-2",
"17-W-2",
"15-W-1",
"11-W-2",
"15-W-2",
"9-W-2",
"6-W-2",
"9-W-1",
"6-W-1",
"5-W-2",
"4-W-2",
"1-W-2",
"5-W-1",
"4-W-1",
"1-W-1",
"7-B-1",
"3-B-1",
"8-B-1",
"2-B-1",
"7-B-2",
"3-B-2",
"8-B-2",
"2-B-2",
"14-B-2",
"14-B-1",
"10-B-2",
"10-B-1",
"12-B-2",
"13-B-2",
"20-B-2",
"18-B-2",
"12-B-1",
"13-B-1",
"20-B-1",
"18-B-1",
"7-R-1",
"3-R-1",
"8-R-1",
"7-R-2",
"3-R-2",
"2-R-1",
"8-R-2",
"2-R-2",
"EYE-R",
"11-R-2",
"11-R-1",
"10-R-2",
"10-R-1",
"12-R-2",
"13-R-2",
"20-R-2",
"18-R-2",
"12-R-1",
"13-R-1",
"20-R-1",
"18-R-1",
"19-G-1",
"16-G-1",
"17-G-1",
"19-G-2",
"16-G-2",
"17-G-2",
"11-G-1",
"11-G-2",
"15-G-1",
"15-G-2",
"EYE-G",
"9-G-2",
"6-G-2",
"9-G-1",
"6-G-1",
"5-G-2",
"4-G-2",
"1-G-2",
"5-G-1",
"4-G-1",
"1-G-1"]

async def configBoard(websocket, path):
    while (1):
        await websocket.send("READY")
        hit = ""
        for i in range(3):

            misses = randint(0, RATIO_MISS)
            if(misses==0):
                hit = "NONE"
            else:
                numberHit =  randint(0, 81)
                hit = sectionsColors[numberHit]

            await websocket.send(hit)
            await asyncio.sleep(TIME_TO_WAIT_HITS)

        for i in range( 0, randint(1,RATIO_REMOVE_DARTS)):
            await websocket.send("REMOVE_DARTS")
            await asyncio.sleep(TIME_TO_WAIT_PLAYERS)


start_server = websockets.serve(configBoard, '127.0.0.1', "9000")
asyncio.get_event_loop().run_until_complete(start_server)
asyncio.get_event_loop().run_forever()
