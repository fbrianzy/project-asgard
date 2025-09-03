// tictactoe_ai.cpp
#include <bits/stdc++.h>
using namespace std;

char board3[9]; // indices 0..8
int lines[8][3] = {{0,1,2},{3,4,5},{6,7,8},{0,3,6},{1,4,7},{2,5,8},{0,4,8},{2,4,6}};

void draw() {
    cout << "\n";
    for (int i=0;i<9;i++){
        cout << (board3[i]==' ' ? char('0'+i) : board3[i]);
        if (i%3!=2) cout << " | ";
        else if (i!=8) cout << "\n---------\n";
    }
    cout << "\n\n";
}

char winner() {
    for (auto &L: lines){
        if (board3[L[0]]!=' ' && board3[L[0]]==board3[L[1]] && board3[L[1]]==board3[L[2]])
            return board3[L[0]];
    }
    return ' ';
}

bool full() { return none_of(begin(board3), end(board3), [](char c){return c==' ';}); }

int minimax(bool aiTurn) {
    char w = winner();
    if (w=='O') return +10;
    if (w=='X') return -10;
    if (full()) return 0;

    int best = aiTurn ? -1e9 : +1e9;
    for (int i=0;i<9;i++){
        if (board3[i]==' '){
            board3[i] = aiTurn ? 'O' : 'X';
            int val = minimax(!aiTurn);
            board3[i] = ' ';
            best = aiTurn ? max(best,val) : min(best,val);
        }
    }
    return best;
}

int bestMove() {
    int move=-1, best=-1e9;
    for (int i=0;i<9;i++){
        if (board3[i]==' '){
            board3[i]='O';
            int val=minimax(false);
            board3[i]=' ';
            if (val>best){best=val; move=i;}
        }
    }
    return move;
}

int main(){
    fill(begin(board3), end(board3), ' ');
    cout << "You are X. AI is O.\n";
    draw();
    while (true){
        int m; cout << "Your move (0-8): "; 
        while (!(cin>>m) || m<0 || m>8 || board3[m]!=' '){ cin.clear(); cin.ignore(1e6,'\n'); cout<<"Invalid. Try: "; }
        board3[m]='X';
        draw();
        if (winner()=='X'){ cout<<"You win!\n"; break; }
        if (full()){ cout<<"Draw!\n"; break; }
        int ai=bestMove();
        board3[ai]='O';
        cout << "AI plays: " << ai << "\n";
        draw();
        if (winner()=='O'){ cout<<"AI wins!\n"; break; }
        if (full()){ cout<<"Draw!\n"; break; }
    }
}
