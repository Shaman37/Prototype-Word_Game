# Prototype-Word_Game
This project is a Unity Example Game from the book [Introduction to Game Design, Prototyping, and Development, 2nd Edition](https://book.prototools.net/), by Jeremy Gibson Bond.

It is a card game prototype, similar to the commercial games [Word Whomp](https://www.pogo.com/games/word-whomp), 
[Jumbline 2](https://play.google.com/store/apps/details?id=com.brainium.jumbline2free&hl=pt_PT&gl=US), 
[Pressed for Words](https://play.google.com/store/apps/details?id=net.aharm.pressed&hl=pt_PT&gl=US) and so on.

## How to play
<p align="center"> 
  <img src="https://user-images.githubusercontent.com/17680666/165864037-9e7a48ab-acc3-45da-a8ad-a3c5d6e5798c.png"/>
</p>

The goal is to guess all the words that are spellable with the available _big letters_ at the bottom part of the screen.
A player can simply type the word and the used letters will be highlighted (_as show on the example below_), 
and delete used letters at will.

If the player is satisfied with her guess, she can press _enter_/_return_ to submit her guess.

At last, a player can hit the _space_ key to rearrange the big letters at the bottom, to help out with the word guessing process.

## Scoring
For every guess, a point is awarded for each letter in the word.

Also, if the word submited as a guess contains any other smaller words, the player also gets points for those plus a multiplier for each word.

Example:

TORNADO -> 7 * 1 (1 point per letter * 1, for being the first word) = 7 points
TORN -> 4 * 2 (1 point per letter * 2, for being the second word) = 8 points
TOR -> 3 * 3 (1 point per letter * 3, for being the third word) = 9 points
ADO -> 3 * 4 (1 point per letter * 4, for being the fourth word) = 12 points

For a total of 36 points!

## Example

https://user-images.githubusercontent.com/17680666/165863989-41565e3b-ed55-4068-b17c-e01b3c538524.mp4
