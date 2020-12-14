namespace Todo.API.UnitTests.Mocks
{
    public static class TokenMock
    {
        private const string TOKEN_SIGNATURE = "rNZQndjf1Bf-11c9A7qELIKEiDPTTFBonflNgBN-cCk";

        public static string USER_98_TOKEN = $"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6InRlc3RAdGVzdC5jb20iLCJuYW1laWQiOiI5OCIsImp0aSI6IjRkNmI4MTEwLTMxODYtNDc3ZC1iMDc5LTVlOWIzMGJkNjNkOCIsInJvbGUiOiJVc2VyIiwiZXhwIjoxNjA1OTY1MDA2fQ.{TOKEN_SIGNATURE}";

        public static string USER_99_TOKEN = $"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6InRlc3RAdGVzdC5jb20iLCJuYW1laWQiOiI5OSIsImp0aSI6IjRkNmI4MTEwLTMxODYtNDc3ZC1iMDc5LTVlOWIzMGJkNjNkOCIsInJvbGUiOiJVc2VyIiwiZXhwIjoxNjA1OTY1MDA2fQ.{TOKEN_SIGNATURE}";
    }
}
