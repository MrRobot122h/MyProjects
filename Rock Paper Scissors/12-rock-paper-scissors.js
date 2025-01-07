
  let score = JSON.parse(localStorage.getItem
  ('score')) || { wins: 0, losses: 0, ties: 0}
  
  //updateScoreElement();

  /*document.querySelector('.js-rock-button') // так чомусь не працює
  .addEventListener('click', () => {
    playGame('rock');
  });*/

  document.addEventListener('DOMContentLoaded', () => {
    document.body.addEventListener('keydown', (event) => {
      if(event.key.toLowerCase() === 'r'){
        playGame('rock');
      } else if(event.key.toLowerCase() === 'p') {
        playGame('paper');
      } else if(event.key.toLowerCase() === 's'){
        playGame('scissors');
      } else if(event.key.toLowerCase() === 'a'){
        autoPlay();
      }

    });
  });
  

  document.addEventListener('DOMContentLoaded', () => {
    document.querySelector('.js-scissors-button').addEventListener('click', () => {
    playGame('scissors');
    });
  });

  document.addEventListener('DOMContentLoaded', () => {
    document.querySelector('.js-paper-button').addEventListener('click', () => {
    playGame('paper');
    });
  });

  document.addEventListener('DOMContentLoaded', () => {
    document.querySelector('.js-rock-button').addEventListener('click', () => {
      playGame('rock');
    });
  });




  function playGame(playerMove){
    const computerMove = pickComputerMove();
    let result = '';
    if(playerMove === 'scissors')
    {
      if(computerMove === 'rock'){
        result = 'You lose.'
      } else if(computerMove === 'paper'){
        result = 'You win.'
      } else if(computerMove === 'scissors'){
        result = 'Tie.';
      }
    } else if(playerMove === 'paper'){
      if(computerMove === 'rock'){
        result = 'You win.'
      } else if(computerMove === 'paper'){
        result = 'Tie.'
      } else if(computerMove === 'scissors'){
        result = 'You lose.';
      }
    } else if(playerMove === 'rock'){
      if(computerMove === 'rock'){
        result = 'Tie.'
      } else if(computerMove === 'paper'){
        result = 'You lose.'
      } else if(computerMove === 'scissors'){
        result = 'You win.';
      }
    }

    if(result === 'You win.') score.wins++;
    else if(result === 'You lose.') score.losses++;
    else if(result === 'Tie.') score.ties++;

    localStorage.setItem('score', JSON.stringify(score));

    updateScoreElement();

    document.querySelector('.js-result')
    .innerHTML = result;

    document.querySelector('.js-moves')
    .innerHTML = `You
  <img src="images/${playerMove}-emoji.png" class="move-icons">
  <img src="images/${computerMove}-emoji.png" class="move-icons">
  Computer`;

  }


  function updateScoreElement(){
    document.querySelector('.js-score')
  .innerHTML =`Wins: ${score.wins}, Losses: ${score.losses}, Ties: ${score.ties}`
  
  }

  function pickComputerMove()
  {
    const randomNumber = Math.random();
    let computerMove = '';

      if(randomNumber >=0 && randomNumber < 1/3) {
        computerMove = 'rock';
      } else if(randomNumber >= 1/3 && randomNumber < 2/3) {
        computerMove = 'paper';
      } else if(randomNumber >= 2/3 && randomNumber < 1){
        computerMove = 'scissors';
      }

    return computerMove;
  }

 let isAutoPlaying = false;
 let intervalId;

 

  function autoPlay(){
   
    if(!isAutoPlaying){
       intervalId = setInterval(() => {
        const playerMove = pickComputerMove();
        playGame(playerMove);
      }, 1000);
     document.querySelector('.js-auto-play').innerHTML = 'Stop auto play';
     isAutoPlaying = true;
    }
    else{
      clearInterval(intervalId);
      document.querySelector('.js-auto-play').innerHTML = 'Auto Play';
      isAutoPlaying = false;
    }
  }
