import { Link } from 'react-router-dom';

const Home = () => {
  return (
    <div>
      <h1>Home Page</h1>
      <nav>
        <Link to="/public">Public Page</Link> | 
        <Link to="/private">Private Page</Link>
      </nav>
    </div>
  );
};

export default Home;
