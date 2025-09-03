#include <iostream>
#include <vector>
#include <fstream>
using namespace std;

struct Task {
    string title;
    bool done;
};

int main() {
    vector<Task> tasks;
    int choice;
    do {
        cout << "\n== To-Do List ==\n";
        cout << "1. Add Task\n2. View Tasks\n3. Mark Done\n4. Save & Exit\nChoice: ";
        cin >> choice;

        if (choice == 1) {
            Task t;
            cout << "Enter task title: ";
            cin.ignore();
            getline(cin, t.title);
            t.done = false;
            tasks.push_back(t);
        } 
        else if (choice == 2) {
            for (int i = 0; i < tasks.size(); i++) {
                cout << i+1 << ". [" << (tasks[i].done ? "x" : " ") << "] " << tasks[i].title << endl;
            }
        } 
        else if (choice == 3) {
            int idx; cout << "Task number: "; cin >> idx;
            if (idx > 0 && idx <= tasks.size()) tasks[idx-1].done = true;
        }
    } while (choice != 4);

    ofstream file("tasks.txt");
    for (auto &t : tasks) file << (t.done ? "1 " : "0 ") << t.title << endl;
    file.close();
    return 0;
}
