﻿using System.Net.Sockets;
using System.Net;
using System.Text;
using CNET.Server;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {


            CancellationTokenSource cts = new CancellationTokenSource();
            MessageServer server = new MessageServer(new MessageFactory()); 
            server.SendMessage("Привет", "user1", "user2");  
            Console.WriteLine("Нажмите любую клавишу для завершения работы сервера");
            Console.ReadKey();
            cts.Cancel();

        }

        // Интерфейс для фабрики сообщений
        public interface IMessageFactory
        {
            Message CreateMessage(string text, string from, string to);
        }

        // Фабрика сообщений
        public class MessageFactory : IMessageFactory
        {
            public Message CreateMessage(string text, string from, string to)
            {
                return new Message { Text = text, DateTime = DateTime.Now, NicknameFrom = from, NicknameTo = to };
            }
        }
        // Интерфейс для наблюдателя
        public interface IObserver
        {
            void Update(Message message);
        }

        // Реализация наблюдателя
        public class MessageObserver : IObserver
        {
            public void Update(Message message)
            {
                Console.WriteLine($"Новое сообщение: {message.DateTime} получено сообщение \"{message.Text}\" от {message.NicknameFrom}");
            }
        }
        public class MessageServer
        {
            private List<IObserver> _observers = new List<IObserver>();
            private IMessageFactory _messageFactory;

            private UdpClient udpServer;

            public MessageServer(IMessageFactory messageFactory)
            {
                _messageFactory = messageFactory;
                udpServer = new UdpClient(12345);
            }

            public void Subscribe(IObserver observer)
            {
                _observers.Add(observer);
            }

            public void Unsubscribe(IObserver observer)
            {
                _observers.Remove(observer);
            }

            public void SendMessage(string text, string from, string to)
            {
                Message newMessage = _messageFactory.CreateMessage(text, from, to);
                NotifyObservers(newMessage);
            }

            private void NotifyObservers(Message message)
            {
                foreach (var observer in _observers)
                {
                    observer.Update(message);
                }
            }
        }
    }
}