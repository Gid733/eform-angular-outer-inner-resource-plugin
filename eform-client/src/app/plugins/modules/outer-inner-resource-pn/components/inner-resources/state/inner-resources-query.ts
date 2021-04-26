import { Injectable } from '@angular/core';
import { Query } from '@datorama/akita';
import {
  InnerResourcesStore,
  InnerResourcesState,
} from './inner-resources-store';

@Injectable({ providedIn: 'root' })
export class InnerResourcesQuery extends Query<InnerResourcesState> {
  constructor(protected store: InnerResourcesStore) {
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
