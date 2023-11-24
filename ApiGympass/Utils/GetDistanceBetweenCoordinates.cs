namespace ApiGympass.Utils
{
    public class GetDistanceBetweenCoordinates
    {
        public static double Calculate(Coordinate from, Coordinate to)
        {
            if (from.Latitude == to.Latitude && from.Longitude == to.Longitude)
            {
                return 0;
            }

            double fromRadian = Math.PI * from.Latitude / 180;
            double toRadian = Math.PI * to.Latitude / 180;

            double theta = from.Longitude - to.Longitude;
            double radTheta = Math.PI * theta / 180;

            double dist = Math.Sin(fromRadian) * Math.Sin(toRadian) +
                          Math.Cos(fromRadian) * Math.Cos(toRadian) * Math.Cos(radTheta);

            if (dist > 1)
            {
                dist = 1;
            }

            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;
            dist = dist * 1.609344;

            return dist;
        }
    }

    public class Coordinate
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}