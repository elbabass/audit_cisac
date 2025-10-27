import * as React from 'react';
import styles from './Home.module.scss';

interface IProps {}

interface IState {
  username: string;
  password: string;
}

class Home extends React.PureComponent<IProps, IState> {
  render() {
    return (
      <div className={styles.container}>
        <div className="container">
          <h1 className="row">Login Form</h1>
          <form className="row" id="userForm" action="/login/localdevelopment" method="post">
            <input type="text" name="userid" defaultValue="imro@cis.net" />
            <input type="text" name="password" defaultValue="cisnet" />
            <input type="text" name="redirecturl" placeholder="/" />
            <button type="submit">Submit</button>
          </form>
        </div>
      </div>
    );
  }
}

export default Home;
