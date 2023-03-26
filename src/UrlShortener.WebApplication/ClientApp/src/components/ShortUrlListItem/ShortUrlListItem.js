import React from 'react';
import styles from './ShortUrlListItem.module.scss';

const ShortUrlListItem = ({ shortUrl, originalUrl }) => (
  <tr>
    <td className={styles.originalUrl} title={originalUrl}>
      {originalUrl}
    </td>
    <td className={styles.shortUrl} title={shortUrl}>
      {shortUrl}
    </td>
  </tr>
);

export default ShortUrlListItem;