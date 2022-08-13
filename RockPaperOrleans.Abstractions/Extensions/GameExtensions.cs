namespace RockPaperOrleans.Abstractions
{
    public static class GameExtensions
    {
        public static string ScoreTurn(this Turn turn)
        {
            // if both players through X, tie game
            if (turn.Throws[0].Play == turn.Throws[1].Play)
                return null;

            // who threw what
            var rock = turn.Throws.FirstOrDefault(x => x.Play == Play.Rock);
            var paper = turn.Throws.FirstOrDefault(x => x.Play == Play.Paper);
            var scissors = turn.Throws.FirstOrDefault(x => x.Play == Play.Scissors);

            // paper covers rock
            if (paper != null && rock != null)
                return paper.Player;

            // rock breaks scissors
            if(rock != null && scissors != null)
                return rock.Player;

            // scissors cut paper
            if(scissors != null && paper != null)
                return scissors.Player;

            // no idea how we'd ever get here
            return null;
        }
    }
}