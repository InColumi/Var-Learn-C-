using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Country country1 = new Country("Страна1");
            Country country2 = new Country("Страна2");

            GenagateSoldiers(country1, country2);

            VarBetweenCountry(country1, country2);
        }

        static void GenagateSoldiers(Country country1, Country country2)
        {
            Random random = new Random();

            country1.AddNewSoldier(new Soldier(random.Next(10, 35), random.Next(10, 75)));
            country1.AddNewSoldier(new Infantryman(random.Next(10, 35), random.Next(10, 75), random.Next(5, 25)));
            country1.AddNewSoldier(new Sniper(random.Next(10, 35), random.Next(10, 75), 0.1f));

            country2.AddNewSoldier(new Soldier(random.Next(10, 35), random.Next(10, 75)));
            country2.AddNewSoldier(new Infantryman(random.Next(10, 35), random.Next(10, 75), random.Next(5, 25)));
            country2.AddNewSoldier(new Sniper(random.Next(10, 35), random.Next(10, 75), 0.1f));
        }

        static void VarBetweenCountry(Country country1, Country country2)
        {
            GenagateSoldiers(country1, country2);
            while(country1.IsAlive && country2.IsAlive)
            {
                country1.ShowInfo();
                country2.ShowInfo();

                country2.TakeDamage(country1.Attack());
                country1.TakeDamage(country2.Attack());

                // Thread.Sleep(1000);
                // Console.Clear();
            }

            if(country1.IsAlive)
            {
                Console.WriteLine("Победила страна: " + country1.Name);
            }
            else if(country2.IsAlive)
            {
                Console.WriteLine("Победила страна: " + country2.Name);
            }
            else
            {
                Console.WriteLine("Ничья");
            }
        }
    }

    class Soldier
    {
        public int Damage { get; private set; }
        public float Health { get; private set; }
        public bool IsAlive => Health > 0;

        public Soldier(int damage, float health)
        {
            Damage = damage;
            Health = health;
        }

        public virtual int Attack()
        {
            return Damage;
        }
        protected bool CheckDamage(int damage)
        {
            if(damage > 0)
            {
                return true;
            }
            return false;
        }

        public virtual void TakeDamage(int damage)
        {
            if(CheckDamage(damage))
            {
                Health -= damage;
            }
        }

        public void ShowInfo()
        {
            Console.WriteLine("HP: " + Health);
        }
    }

    class Infantryman:Soldier
    {
        public int Armor { get; private set; }
        public Infantryman(int damage, float health, int armor) : base(damage, health)
        {
            Armor = armor;
        }

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage - Armor);
        }
    }

    class Sniper:Soldier
    {
        private Random _random;
        private float _percentOfDamage;
        public int StrongShot { get; private set; }
        public Sniper(int damage, float health, float percentOfDamage) : base(damage, health)
        {
            _random = new Random();
            _percentOfDamage = percentOfDamage;
            StrongShot = Convert.ToInt32(damage * _percentOfDamage);
        }

        public override int Attack()
        {
            if(_random.Next(1, 11) <= 3)
            {
                return base.Attack() + StrongShot;
            }
            else
            {
                return base.Attack();
            }
        }
    }

    class Country
    {
        public string Name { get; private set; }
        public bool IsAlive => _platoon.Count > 0;

        private Random _random;

        private List<Soldier> _platoon;

        public Country(string name)
        {
            Name = name;
            _platoon = new List<Soldier>();
            _random = new Random();
        }

        public void AddNewSoldier(Soldier soldier)
        {
            _platoon.Add(soldier);
        }

        public int Attack()
        {
            if(IsAlive)
            {
                return _platoon[_random.Next(0, _platoon.Count)].Attack();
            }
            return 0;
        }

        public void TakeDamage(int damage)
        {
            if(_platoon.Count > 0)
            {
                int randomSoldier = _random.Next(0, _platoon.Count);
                _platoon[randomSoldier].TakeDamage(damage);
            }
            DeleteDeadSoldier();
        }

        private void DeleteDeadSoldier()
        {
            for(int i = 0; i < _platoon.Count; i++)
            {
                if(_platoon[i].IsAlive == false)
                {
                    _platoon.RemoveAt(i);
                }
            }
        }

        public void ShowInfo()
        {
            Console.WriteLine($"Страна - {Name}. Количество солдат: {_platoon.Count}.");
            Console.WriteLine("Состойние солдат:");
            foreach(var soldier in _platoon)
            {
                soldier.ShowInfo();
            }
        }
    }
}