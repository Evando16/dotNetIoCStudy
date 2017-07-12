using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoCContainerStart
{
    class Program
    {
        static void Main(string[] args)
        {
            //ICreditCard creditCard = new MasterCard(); ;
            //ICreditCard otherCard = new Visa();
            //var shopper = new Shopper(creditCard);
            //shopper.Charge();

            //var otherShopper = new Shopper(otherCard);
            //otherShopper.Charge();

            //Resolver resolver = new Resolver();
            //var shopper3 = new Shopper(resolver.ResolveCreditCard());
            //shopper3.Charge();


            Resolver resolver = new Resolver();

            resolver.Register<Shopper,Shopper>();
            resolver.Register<ICreditCard, MasterCard>();
            //Resolver resolver = new Resolver();

            var shooper = resolver.Resolve<Shopper>();
            shooper.Charge();

            Console.Read();
        }
    }

    public class Resolver
    {
        //public ICreditCard ResolveCreditCard()
        //{
        //    if (new Random().Next(2) == 1)
        //    {
        //        return new Visa();
        //    }

        //    return new MasterCard();
        //}

        private Dictionary<Type, Type> dependencyMap = new Dictionary<Type, Type>();

        public T Resolve<T>()
        {
            return (T)Resolve(typeof (T));
        }

        public void Register<TFrom, TTo>()
        {
            dependencyMap.Add(typeof(TFrom), typeof(TTo));
        }

        private object Resolve(Type typeToResolve)
        {
            Type resolvedType = null;

            try {
                resolvedType = dependencyMap[typeToResolve];
            }
            catch {
                throw new Exception(string.Format("Could not resolve type {0}", typeToResolve.FullName));
            }

            var firstContructor = resolvedType.GetConstructors().First();
            var contructorParameters = firstContructor.GetParameters();

            if (contructorParameters.Count() == 0)
                return Activator.CreateInstance(resolvedType);

            IList<object> parameters = new List<object>();
            foreach (var parameterToResolve in contructorParameters)
            {
                parameters.Add(Resolve(parameterToResolve.ParameterType));
            }
            return firstContructor.Invoke(parameters.ToArray());
        }
    }

    public class Visa : ICreditCard
    {
        public string Charge()
        {
            return "Charging the Visa!";
        }
    }

    public class MasterCard : ICreditCard
    {
        public string Charge()
        {
            return "Swiping the MasterCard";
        }
    }

    public class Shopper
    {
        private readonly ICreditCard _creditCard;

        public Shopper(ICreditCard creditCard)
        {
            this._creditCard = creditCard;
        }

        public void Charge()
        {
            var chargeMessage = _creditCard.Charge();
            Console.WriteLine(chargeMessage);
        }
    }

    public interface ICreditCard
    {
        string Charge();
    }
}
