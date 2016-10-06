using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphDetectCycles
{
    
    class Link<T>
    {
        public T Data { get; set; }
        public List<T> Reference { get; set; }

        public List<T> AssignedData { get; set; }
    }

    public class Vertex<T>
    {
        public T Data { get; set; }
        public List<Vertex<T>> Parents { get; set; }
        public List<Vertex<T>> Childs { get; set; }

        public List<Vertex<T>> Neighbors
        {
            get
            {
                return Parents.Union(Childs).ToList();
            }
        }

        public Vertex()
        {
            Parents = new List<Vertex<T>>();
            Childs = new List<Vertex<T>>();
        }

        public override string ToString()
        {
            return Data.ToString();
        }
    }

    public class Graph<T>
    {
        
        private List<Vertex<T>> Vertices { get; set; }

        public Graph()
        {
            Vertices = new List<Vertex<T>>();
        }

        private Vertex<T> GetVertex(T data)
        {
            var vertex = Vertices.FirstOrDefault(v => v.Data.Equals(data));
            if (vertex == null)
            {
                vertex = new Vertex<T> { Data = data };
                Vertices.Add(vertex);
            }

            return vertex;
        }

        public void AddVertex(T data, List<T> references = null)
        {
            var vertex = GetVertex(data);

            if (references != null)
            {
                foreach (var r in references)
                {
                    if (!vertex.Parents.Any(n => n.Data.Equals(r)))
                    {
                        var parent = GetVertex(r);
                        if (!parent.Childs.Contains(vertex))
                        {
                            parent.Childs.Add(vertex);
                        }

                        vertex.Parents.Add(parent);
                    }
                }
            }
        }

        public List<List<Vertex<T>>> DetectCycle()
        {
            var visited = new List<Vertex<T>>();
            var recStack = new List<Vertex<T>>();
            var cycles = new List<List<Vertex<T>>>();
            foreach(var vertex in Vertices)
            {
                DetectCycle(vertex, visited, recStack, cycles);
            }

            return cycles;
        }

        private void DetectCycle(Vertex<T> vertex , List<Vertex<T>> visited, List<Vertex<T>> recStack, List<List<Vertex<T>>> cycles)
        {
            visited.Add(vertex);

            recStack.Add(vertex);
            foreach(var child in vertex.Childs)
            {
                if (!visited.Contains(child))
                {
                    DetectCycle(child, visited, recStack, cycles);
                }
                else if (recStack.Contains(child))
                {
                    var id = recStack.IndexOf(child);
                    var cycle = recStack.GetRange(id, recStack.Count - id);
                    if (!cycles.Any(c => c.SequenceEqual(cycle)))
                    {
                        cycles.Add(cycle);
                    }
                }
            }
            recStack.Remove(vertex);
        }
    }
    
    class Program
    {
        static void DetectCycles<T>(List<Link<T>> links)
        {
            var graph = new Graph<T>();

            foreach (var link in links)
            {
                graph.AddVertex(link.Data, link.Reference);
            }

            var cycles = graph.DetectCycle();

            if (cycles.Any())
            {
                Console.WriteLine(cycles.Count + " cycle(s) detected : ");
                foreach(var cycle in cycles)
                {
                    Console.WriteLine(string.Join(",", cycle.Select(v => v.Data)));
                }
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Figure 1 : ");
            DetectCycles(new List<Link<string>>
            {
                new Link<string> {Data = "A",  Reference = new List<string> { "C" } },
                new Link<string> {Data = "B",  Reference = new List<string> { "A" } },
                new Link<string> {Data = "C",  Reference = new List<string> { "B" } },
            });

            Console.WriteLine("\nFigure 2 : ");
            DetectCycles(new List<Link<string>>
            {
                new Link<string> {Data = "A",  Reference = new List<string> { } },
                new Link<string> {Data = "B",  Reference = new List<string> { "A" } },
                new Link<string> {Data = "C",  Reference = new List<string> { "A", "B" } },
            });

            Console.WriteLine("\nFigure 3 : ");
            DetectCycles(new List<Link<string>>
            {
                new Link<string> {Data = "A",  Reference = new List<string> { } },
                new Link<string> {Data = "B",  Reference = new List<string> { "A" } },
                new Link<string> {Data = "C",  Reference = new List<string> { "A", "B" } },
                new Link<string> {Data = "D",  Reference = new List<string> { "A", "C", "E" } },
                new Link<string> {Data = "E",  Reference = new List<string> { "A" } },
            });

            Console.WriteLine("\nFigure 4 : ");
            DetectCycles(new List<Link<string>>
            {
                new Link<string> {Data = "A",  Reference = new List<string> { } },
                new Link<string> {Data = "B",  Reference = new List<string> { "A" } },
                new Link<string> {Data = "C",  Reference = new List<string> { "B" } },
                new Link<string> {Data = "D",  Reference = new List<string> { "C" } },
                new Link<string> {Data = "E",  Reference = new List<string> { "C", "H" } },
                new Link<string> {Data = "F",  Reference = new List<string> { "E" } },
                new Link<string> {Data = "G",  Reference = new List<string> { "F" } },
                new Link<string> {Data = "H",  Reference = new List<string> { "G" } },
                new Link<string> {Data = "H",  Reference = new List<string> { "G" } },
                new Link<string> {Data = "I",  Reference = new List<string> { "D", "I" } },
            });

            Console.WriteLine("\nFigure 5 : ");
            DetectCycles(new List<Link<string>>
            {
                new Link<string> {Data = "A",  Reference = new List<string> { } },
                new Link<string> {Data = "B",  Reference = new List<string> { "A", "E" } },
                new Link<string> {Data = "C",  Reference = new List<string> { "B", "C" } },
                new Link<string> {Data = "D",  Reference = new List<string> { "B" } },
                new Link<string> {Data = "E",  Reference = new List<string> { "D" } },
            });

            Console.WriteLine("\nFigure 6 : ");
            DetectCycles(new List<Link<string>>
            {
                new Link<string> {Data = "A",  Reference = new List<string> { } },
                new Link<string> {Data = "B",  Reference = new List<string> { "A" } },
                new Link<string> {Data = "C",  Reference = new List<string> { } },
                new Link<string> {Data = "D",  Reference = new List<string> { } },
                new Link<string> {Data = "E",  Reference = new List<string> { "C", "H" } },
                new Link<string> {Data = "F",  Reference = new List<string> { "E" } },
                new Link<string> {Data = "G",  Reference = new List<string> { "F" } },
                new Link<string> {Data = "H",  Reference = new List<string> { "G" } },
                new Link<string> {Data = "H",  Reference = new List<string> { "G" } },
                new Link<string> {Data = "I",  Reference = new List<string> { "D", "I" } },
            });

            Console.WriteLine("\nFigure 7 : ");
            DetectCycles(new List<Link<string>> {
                new Link<string> { Data = "B.val1", Reference = new List<string> { "A.val1" } },
            });

            Console.ReadLine();
        }
    }
    
}
