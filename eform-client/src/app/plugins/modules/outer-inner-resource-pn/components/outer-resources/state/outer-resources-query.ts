import { Injectable } from '@angular/core';
import { Query } from '@datorama/akita';
import {
  OuterResourcesStore,
  OuterResourcesState,
} from './outer-resources-store';

@Injectable({ providedIn: 'root' })
export class OuterResourcesQuery extends Query<OuterResourcesState> {
  constructor(protected store: OuterResourcesStore) {
    super(store);
  }

  get pageSetting() {
    return this.getValue();
  }

  selectPageSize$ = this.select('pageSize');
  selectIsSortDsc$ = this.select('isSortDsc');
  selectSort$ = this.select('sort');
  selectOffset$ = this.select('offset');
}
