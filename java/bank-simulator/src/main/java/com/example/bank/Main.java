package com.example.bank;

import java.util.*;
import java.util.concurrent.*;
import java.util.concurrent.locks.ReentrantLock;

class Account {
    final int id; long balance;
    private final ReentrantLock lock = new ReentrantLock(true);
    Account(int id, long balance){ this.id=id; this.balance=balance; }
    void lock(){ lock.lock(); }
    void unlock(){ lock.unlock(); }
    void deposit(long amt){ balance += amt; }
    void withdraw(long amt){ balance -= amt; }
}

public class Main {
    static void transfer(Account from, Account to, long amt){
        Account first = from.id < to.id ? from : to;
        Account second = from.id < to.id ? to : from;
        first.lock(); second.lock();
        try {
            if(from.balance >= amt){
                from.withdraw(amt);
                to.deposit(amt);
            }
        } finally { second.unlock(); first.unlock(); }
    }

    public static void main(String[] args) throws InterruptedException {
        int N = 5; int TX = 1000;
        List<Account> accs = new ArrayList<>();
        for(int i=0;i<N;i++) accs.add(new Account(i, 1_000_000));

        long startTotal = accs.stream().mapToLong(a->a.balance).sum();
        ExecutorService pool = Executors.newFixedThreadPool(8);
        Random rnd = new Random();

        for(int i=0;i<TX;i++){
            pool.submit(() -> {
                Account a = accs.get(rnd.nextInt(N));
                Account b = accs.get(rnd.nextInt(N));
                if(a==b) return;
                transfer(a, b, rnd.nextInt(5000));
            });
        }
        pool.shutdown(); pool.awaitTermination(10, TimeUnit.SECONDS);

        long endTotal = accs.stream().mapToLong(a->a.balance).sum();
        System.out.println("Total before: " + startTotal + " | after: " + endTotal);
        for(Account a: accs) System.out.printf("Acc #%d balance = %d%n", a.id, a.balance);
    }
}
