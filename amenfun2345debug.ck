class Keyboard {
    
    KBHit kb;
    0 => int prev;
    0 => int next;
    0 => int back;
    0 => int rand;
    0 => int swap;
    0 => int canWriteSelect;
    0 => int canWritePosition;
    int lastNavVal;
    int lastSelectVal;
    int lastPositionVal;
    int tempVal;
    
    fun void listen()
    {
        while( true )
        {
            kb => now;
            <<<"A">>>;
            while( kb.more() ) { kb.getchar() => tempVal; }
            
            // pressing "[" selects the previous sequence in playlist
            if (tempVal == 91) { 
                1 => prev; 
                0 => next;
            }
            // pressing "]" selects the next sequence in playlist
            if (tempVal == 93) { 
                0 => prev;
                1 => next;
            }
            // pressing delete reverts to the last used sequence
            if (tempVal == 127) { 1 => back; }
            // pressing spacebar creates a randomized sequence
            if (tempVal == 32) { 1 => rand; }
            // pressing tab swaps current sequence into playlist
            if (tempVal == 9) { 1 => swap; }
            // pressing "-" writes last pressed position value in step
            if (tempVal == 45) { 1 => canWritePosition; }
            // pressing "=" writes last pressed select value in step
            if (tempVal == 61) { 1 => canWriteSelect; }
            // pressing numbers 1 - 8 sets lastPositionVal up to write
            if ((tempVal > 48) && (tempVal < 57)) { 
                (tempVal - 49) => lastPositionVal; 
            }
            // pressing letters a - p sets lastSelectVal up to write
            if ((tempVal > 96) && (tempVal < 113)) { 
                    (tempVal - 97) => lastSelectVal;
            }
        }        
    }
    
    spork ~ listen();
    
    fun int getPrevious () 
    {<<<"B">>>;
        if (prev) { 
            0 => prev;
            return 1; 
        }
        else { return 0; }
    }
    
    fun int getNext () 
    {<<<"C">>>;
        if (next) { 
            0 => next;
            return 1; 
        }
        else { return 0; }
    }
    
    fun int getBack () 
    {<<<"D">>>;
        if (back) { 
            0 => back;
            return 1; 
        }
        else { return 0; }
    }
    
    fun int getRandom () 
    {<<<"E">>>;
        if (rand) { 
            0 => rand;
            return 1; 
        }
        else { return 0; }
    }
    
    fun int swapIn () 
    {<<<"F">>>;
        if (swap) { 
            0 => swap;
            return 1; 
        }
        else { return 0; }
    }
    
    fun int getLastSelect () 
    {<<<"G">>>;
        if (canWriteSelect) { 
            0 => canWriteSelect;
            <<<"g">>>;
            return lastSelectVal; 
        }
        else { <<<"g">>>; return -1; }
    }
    
    fun int getLastPosition () 
    {<<<"H">>>;
        if (canWritePosition) { 
            0 => canWritePosition;
            <<<"h">>>;
            return lastPositionVal; 
        }
        else { <<<"h">>>; return -1; }
    }
    
}

class Server {
    
    // select seed preset list (slot 0 stores last played sequence)
    [ [15,1,9,9,1,5,10,5],
      [15,1,9,9,1,5,10,5],
      [0,4,4,4,4,9,4,4],
      [0,4,4,4,4,9,4,4],
      [2,1,12,9,14,5,10,5],
      [11,14,13,3,1,14,14,7],
      [7,8,14,13,7,7,2,7],
      [3,14,8,0,4,10,4,1],
      [10,12,10,0,1,12,3,10],
      [12,8,1,11,13,3,9,5],
      [2,2,1,13,2,15,2,7] ] @=> int selectPack[][];
      
    // position seed preset list (remaining slots are for playlist)
    [ [0,1,2,0,6,7,3,7],
      [0,1,2,0,6,7,3,7],
      [3,1,4,1,7,2,4,2],
      [3,1,4,1,5,9,2,6],
      [6,2,3,6,1,6,1,1],
      [1,5,3,5,0,7],
      [1,5,1,4,5,7],
      [2,2,5,4,1],
      [5,0,6,0,3],
      [3,1,2,5,5],
      [7,3,4,4,5] ] @=> int positionPack[][];
      
      1 => int scrolldex;
      [[15,1,9,9,1,5,10,5],[0,1,2,0,6,7,3,7]] @=> int returnPack[][];
      
      fun void scrollLeft(int sp[][]) 
      {<<<"I">>>;   
          sp[0] @=> selectPack[0];
          sp[1] @=> positionPack[0];
          scrolldex--;
          if (scrolldex < 1) { 1 => scrolldex; }
          selectPack[scrolldex] @=> returnPack[0];
          positionPack[scrolldex] @=> returnPack[1];
      }
      
      fun void scrollRight(int sp[][]) 
      { <<<"J">>>;  
          sp[0] @=> selectPack[0];
          sp[1] @=> positionPack[0];
          scrolldex++;
          if (scrolldex > (selectPack.cap()-1)) 
              { (selectPack.cap()-1) => scrolldex; }
          selectPack[scrolldex] @=> returnPack[0];
          positionPack[scrolldex] @=> returnPack[1];
      }
      
      fun void goBack(int sp[][]) 
      {<<<"K">>>;
          selectPack[0] @=> int seltemp[];
          positionPack[0] @=> int postemp[]; 
          sp[0] @=> selectPack[0];
          sp[1] @=> positionPack[0];       
          [seltemp, postemp] @=> returnPack;;
      }
      
      fun void swapInPlace(int sp[][])
      {<<<"L">>>;
          sp[0] @=> selectPack[scrolldex];
          sp[1] @=> positionPack[scrolldex];
      }
      
}

class Sequencer {
    
    Keyboard kb;
    Server servo;
    
    // initial seeds, can be altered to taste, capacity determines step #
    [15,1,9,9,1,5,10,5] @=> int select[];
    [0,1,2,0,6,7,3,7] @=> int position[];
    // ^--(they get overwritten in real time using above-mentioned keys)
    
    0 => int seldex;
    0 => int posdex;
    1 => int scrolldex;
    int beatLength;
    
    fun void randomize() 
    {   <<<"M">>>;
        select @=> servo.selectPack[0];
        position @=> servo.positionPack[0];
        int seltemp[select.cap()];
        int postemp[position.cap()];
        for (0 => int i; i < seltemp.cap(); i++) {
            Math.random2(0,15) => seltemp[i];
        }
        for (0 => int i; i < postemp.cap(); i++) {
            Math.random2(0,7) => postemp[i];
        }        
        seltemp @=> select;
        postemp @=> position;
    }
 
    fun int getNextSelect() 
    {<<<"N">>>;
        kb.getLastSelect() => int lstemp;
        seldex => int val;
        seldex++;
        // maybe also this fucker...
        if (seldex >= select.cap()) { 0 => seldex; }
        if (lstemp != -1) { lstemp => select[val]; }
        return select[val]; 
    }
          
    fun int getNextPosition() 
    {<<<"O">>>;
        kb.getLastPosition() => int lptemp;
        posdex => int val;
        posdex++;
        // but definitely this fucker right here needed to be >=, not ==
        if (posdex >= position.cap()) { 0 => posdex; }
        if (lptemp != -1) { lptemp => position[val]; }
        <<<"o: "+val>>>;
        return position[val]; 
    }
    
    fun void printSelectArray() 
    {<<<"P">>>;
        chout <= "| ";
        for (0 => int i; i < select.cap(); i++) {
            chout <= select[i] <= " | ";
        }
        chout <= IO.newline();
    }
    
    fun void printPositionArray() 
    {<<<"Q">>>;
        chout <= "| ";
        for (0 => int i; i < position.cap(); i++) {
            chout <= position[i] <= " | ";
        }
        chout <= IO.newline();
    }
    
    fun void checkNav()
    {<<<"R">>>;
        while (true) {
            0 => int update;
            if (kb.getPrevious()) { 
                spork ~ servo.scrollLeft([select, position]);
                1 => update;
            }
            if (kb.getNext()) { 
                spork ~ servo.scrollRight([select, position]);
                1 => update;
            }
            if (kb.getBack()) { 
                spork ~ servo.goBack([select, position]);
                1 => update;
            }
            if (kb.swapIn()) { 
                spork ~ servo.swapInPlace([select, position]); 
            }
            (0.8*beatLength)::samp => now;
            if (update) {
                servo.returnPack[0] @=> select;
                servo.returnPack[1] @=> position;
            }
        }
    }
    
    spork ~ checkNav();
    
}

16 => int channels;
SndBuf amen[channels];
Pan2 pan[channels];
JCRev rev1[channels];
NRev rev2[channels];
0 => int allsamples;
Sequencer seq;

for (0 => int i; i < channels; i++)
{
    amen[i] => rev1[i];
    // BYOL(bring your own loops), wants 16 of them labeled 0.wav - 15.wav
    me.dir()+"/audiobin/amen/loops/"+i+".wav" => amen[i].read;
    // ^-- (all samples must have the same length and tempo)
    amen[i].samples() +=> allsamples;
    amen[i].samples() => amen[i].pos;

    rev1[i] => rev2[i] => pan[i] => dac;
    0.0 => rev1[i].mix;
    0.0 => rev2[i].mix;
}

16 /=> allsamples;
allsamples/2 => int bar;
bar/4 => int beat;
beat => seq.beatLength;

fun void playAmen(float gain, float rate, float tempo) 
{
    while (true)
    {<<<"S">>>;
        seq.getNextSelect() % 16 => int selector;
        seq.getNextPosition() % 8 => int position;
        <<<"problem child">>>;
        amen[selector].samples() => int length;
        beat*position => int cursor;
        if ((cursor+tempo) < length) { cursor => amen[selector].pos; }
        <<<"right here">>>;
        gain => amen[selector].gain;
        rate => amen[selector].rate;
        Math.random2f(-0.75,0.75) => pan[selector].pan;
        Math.random2f(0.00,0.05) => rev1[selector].mix;
        Math.random2f(0.00,0.05) => rev2[selector].mix;
        now + tempo::samp => time later;
        while( now < later )
        {
            if (cursor == amen[selector].samples()) {
                0 => amen[selector].pos;
            }           
            1::samp => now;
            cursor++;
            // UN-FUCKING-SOLVEABLE
        }
        amen[selector].samples() => amen[selector].pos;
        <<<"s">>>;
    }
}

fun void printArrays(Sequencer s) 
{
    while (true) {<<<"T">>>;
        s.printSelectArray();
        s.printPositionArray();
        chout <= IO.newline();
        0.25::second => now; // can refine display refresh period to taste
    }
}

fun void mixItUp() 
{
    while (true) {<<<"U">>>;
        //0 => int token;
        (0.8*beat)::samp => now;
        if (seq.kb.getRandom()) { seq.randomize(); }
        //else { token++; }
        //(0.8*allsamples)::samp => now;
        //if (seq.kb.wantNext()) { seq.randomize(); }
        //else { token++; }
        //(0.8*allsamples)::samp => now;
        //if (seq.kb.wantNext()) { seq.randomize(); }
        //else { token++; }
        //(0.8*allsamples)::samp => now;
        //if (seq.kb.wantNext()) { seq.randomize(); }
        //else { token++; }
        //(0.8*allsamples)::samp => now;
        //if (seq.kb.wantNext()) { seq.randomize(); }
        //else { token++; }
        //(0.8*allsamples)::samp => now;
        //if (seq.kb.wantNext()) { seq.randomize(); }
        //else { token++; }
        //(0.8*allsamples)::samp => now;
        //if (seq.kb.wantNext()) { seq.randomize(); }
        //else { token++; }
        //(0.8*allsamples)::samp => now;
        //if (token > 6) seq.randomize();
    }   
}

// modifications to rate require inverse ratio as multiplier to tempo
spork ~ playAmen(1.0, 1.25, ((0.8)*bar));
spork ~ playAmen(0.75, 1.25, ((0.8)*(bar/2)));
spork ~ playAmen(0.50, 1.25, ((0.8)*beat));
spork ~ playAmen(0.25, 1.25, ((0.8)*(beat/2)));

spork ~ printArrays(seq);

// turn randomizer on and off
spork ~ mixItUp();

while( true ) // keep the fire alive
    1::second => now;