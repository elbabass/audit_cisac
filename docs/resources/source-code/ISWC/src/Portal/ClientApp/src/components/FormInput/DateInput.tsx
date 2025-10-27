import React, { PureComponent } from 'react';
import styles from './FormInput.module.scss';
import { IDateInputProps, IDateInputState } from './FormInputTypes';

export default class DateInput extends PureComponent<IDateInputProps, IDateInputState> {
  constructor(props: IDateInputProps) {
    super(props);
    this.state = {
      date: props.value,
    };
  }
  componentDidMount() {
    const { value } = this.props;
    let date = value && value.slice(0, 10);
    this.setState({ date });
  }
  componentDidUpdate() {
    const { value } = this.props;
    let date = value && value.slice(0, 10);
    this.setState({ date });
  }
  render() {
    const { label, changeData, name } = this.props;
    const { date } = this.state;

    return (
      <div className={styles.container}>
        <div className={styles.formLabel}>{label}</div>
        <div className={styles.dateContainer}>
          <input
            type={'date'}
            className={styles.dateInput}
            value={date}
            id={name}
            onChange={(event) => changeData && changeData(event)}
            placeholder={'dd/mm/yyyy'}
          />
        </div>
      </div>
    );
  }
}
