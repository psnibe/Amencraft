class Keyboard {
    
    KBHit kb;
    0 => int prev;
    0 => int next;
    0 => int back;
    0 => int rand;
    0 => int swap;
    0 => int canWriteSelect;
    0 => int canWritePosition;
    int lastSelectVal;
    int lastPositionVal;
    int tempVal;
    
    fun void listen()
    {
        while( true )
        {
            kb => now;
            
            while( kb.more() ) { kb.getchar() => tempVal; }
            
            // pressing "[" selects the previous sequence in playlist
            if (tempVal == 91) { 1 => prev; }
            // pressing "]" selects the next sequence in playlist
            if (tempVal == 93) { 1 => next; }
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
    {
        if (prev) { 
            0 => prev;
            return 1; 
        }
        else { return 0; }
    }
    
    fun int getNext () 
    {
        if (next) { 
            0 => next;
            return 1; 
        }
        else { return 0; }
    }
    
    fun int getBack () 
    {
        if (back) { 
            0 => back;
            return 1; 
        }
        else { return 0; }
    }
    
    fun int getRandom () 
    {
        if (rand) { 
            0 => rand;
            return 1; 
        }
        else { return 0; }
    }
    
    fun int swapIn () 
    {
        if (swap) { 
            0 => swap;
            return 1; 
        }
        else { return 0; }
    }
    
    fun int getLastSelect () 
    {
        if (canWriteSelect) { 
            0 => canWriteSelect;
            return lastSelectVal; 
        }
        else { return -1; }
    }
    
    fun int getLastPosition () 
    {
        if (canWritePosition) { 
            0 => canWritePosition;
            return lastPositionVal; 
        }
        else { return -1; }
    }
    
}

class Sequencer {
    
    Keyboard kb;
    
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
    
    // initial seeds, can be altered to taste, capacity determines step #
    selectPack[0] @=> int select[];
    positionPack[0] @=> int position[];
    // ^--(they get overwritten in real time using above-mentioned keys)
    
    0 => int seldex;
    0 => int posdex;
    1 => int scrolldex;
    
    fun void scrollLeft() 
    {   
        select @=> selectPack[0];
        position @=> positionPack[0];
        scrolldex--;
        if (scrolldex < 1) { 1 => scrolldex; }
        selectPack[scrolldex] @=> select;
        positionPack[scrolldex] @=> position;
    }
    
    fun void scrollRight() 
    {   
        select @=> selectPack[0];
        position @=> positionPack[0];
        scrolldex++;
        if (scrolldex > (selectPack.cap()-1)) 
            { (selectPack.cap()-1) => scrolldex; }
        selectPack[scrolldex] @=> select;
        positionPack[scrolldex] @=> position;
    }
    
    fun void goBack() 
    {   
        select @=> int seltemp[];
        position @=> int postemp[];       
        selectPack[0] @=> select;
        positionPack[0] @=> position;
        seltemp @=> selectPack[0];
        postemp @=> positionPack[0];
    }
    
    fun void randomize() 
    {   
        select @=> selectPack[0];
        position @=> positionPack[0];
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
    
    fun void swapInPlace()
    {
        select @=> selectPack[scrolldex];
        position @=> positionPack[scrolldex];
    }
 
    fun int getNextSelect() 
    {
        kb.getLastSelect() => int lstemp;
        seldex => int val;
        seldex++;
        if (seldex == select.cap()) { 0 => seldex; }
        if (lstemp != -1) { lstemp => select[val]; }
        return select[val]; 
    }
          
    fun int getNextPosition() 
    {
        kb.getLastPosition() => int lptemp;
        posdex => int val;
        posdex++;
        if (posdex == position.cap()) { 0 => posdex; }
        if (lptemp != -1) { lptemp => position[val]; }
        return position[val]; 
    }
    
    fun void printSelectArray() 
    {
        chout <= "| ";
        for (0 => int i; i < select.cap(); i++) {
            chout <= select[i] <= " | ";
        }
        chout <= IO.newline();
    }
    
    fun void printPositionArray() 
    {
        chout <= "| ";
        for (0 => int i; i < position.cap(); i++) {
            chout <= position[i] <= " | ";
        }
        chout <= IO.newline();
    }
    
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

fun void playAmen(float gain, float rate, float tempo) 
{
    while (true)
    {
        seq.getNextSelect() => int selector;
        beat*(seq.getNextPosition()) => amen[selector].pos;
        gain => amen[selector].gain;
        rate => amen[selector].rate;
        Math.random2f(-0.75,0.75) => pan[selector].pan;
        Math.random2f(0.00,0.05) => rev1[selector].mix;
        Math.random2f(0.00,0.05) => rev2[selector].mix;
        tempo::samp => now;
        amen[selector].samples() => amen[selector].pos;
    }
}

fun void printArrays(Sequencer s) 
{
    while (true) {
        s.printSelectArray();
        s.printPositionArray();
        chout <= IO.newline();
        0.25::second => now; // can refine display refresh period to taste
    }
}

fun void checkNav()
{
    while (true) {
        (0.8*beat)::samp => now;
        if (seq.kb.getPrevious()) { seq.scrollLeft(); }
        if (seq.kb.getNext()) { seq.scrollRight(); }
        if (seq.kb.getBack()) { seq.goBack(); }
        if (seq.kb.swapIn()) { seq.swapInPlace(); }
    }
}

fun void mixItUp() 
{
    while (true) {
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
spork ~ checkNav();

// turn randomizer on and off
spork ~ mixItUp();

while( true ) // keep the fire alive
    1::second => now;